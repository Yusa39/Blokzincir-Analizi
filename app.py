#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
app.py
------
Streamlit arayüz katmanı — sadece UI kodu içerir.

Modül Bağımlılık Ağacı
-----------------------
app.py
  ├── blokzincir.py  →  BlockchainNetwork, FundFlowAnalyzer, SyntheticDataGenerator
  │     ├── islem.py     →  Transaction
  │     └── dugum.py     →  Wallet
  └── merkle_agaci.py →  MerkleTree
        └── islem.py     →  Transaction

Çalıştırmak için:
    python -m streamlit run app.py
"""

import os
import json
import random
import tempfile

import streamlit as st
import streamlit.components.v1 as components
import pandas as pd
from pyvis.network import Network

# Proje modülleri
from islem import Transaction
from dugum import Wallet
from merkle_agaci import MerkleTree
from blokzincir import BlockchainNetwork, FundFlowAnalyzer, SyntheticDataGenerator


# =============================================================
# GÖRSELLEŞTİRME YARDIMCI FONKSİYONLARI
# =============================================================

def _normalize(value: float, min_v: float, max_v: float) -> float:
    """Değeri 0–1 aralığına normalize eder."""
    span = max_v - min_v
    if span == 0:
        return 0.5
    return (value - min_v) / span


def build_pyvis_html(
    network: BlockchainNetwork,
    highlighted_nodes: list[str] | None = None,
    highlighted_edges: list[tuple] | None = None,
) -> str:
    """
    NetworkX yönlü grafından PyVis interaktif HTML üretir.

    Görsel kurallar
    ---------------
    - Düğüm boyutu   : bakiyeyle orantılı (12–55 px)
    - Kenar kalınlığı: transfer toplamıyla orantılı (1–9 px)
    - Vurgulanan     : turuncu arka plan + altın kenarlık
    - Normal         : mavi tonları (bakiyeye göre koyu→açık)

    Returns
    -------
    Tarayıcıda gösterilebilir HTML string'i
    """
    h_nodes: set[str] = set(highlighted_nodes or [])
    h_edges: set[tuple] = set(map(tuple, highlighted_edges or []))

    # PyVis ağı — koyu arka plan
    net = Network(
        height="620px",
        width="100%",
        directed=True,
        bgcolor="#0e1117",
        font_color="white",
    )

    # Bakiye sınırlarını hesapla (düğüm boyutu normalizasyonu için)
    balances = [
        network.wallets[n].balance
        for n in network.graph.nodes()
        if n in network.wallets
    ]
    min_b = min(balances) if balances else 0.0
    max_b = max(balances) if balances else 1.0

    # ---- Düğümleri ekle ----
    for node in network.graph.nodes():
        wallet = network.wallets.get(node)
        balance = wallet.balance if wallet else 0.0
        norm = _normalize(balance, min_b, max_b)

        # Boyutu bakiyeye göre ölçekle: 12–55 piksel
        size = 12 + norm * 43

        if node in h_nodes:
            # Vurgulanan: altın kenarlı turuncu
            node_color = {
                "background": "#FF6B35",
                "border": "#FFD700",
                "highlight": {"background": "#FF8C55", "border": "#FFD700"},
            }
        else:
            # Normal: mavi tonları (bakiyeye göre koyudan açığa)
            blue = int(80 + norm * 175)
            hex_blue = f"#{30:02x}{80:02x}{blue:02x}"
            node_color = {
                "background": hex_blue,
                "border": "#4A90D9",
                "highlight": {"background": "#5AAFFB", "border": "#FFFFFF"},
            }

        incoming_cnt = len(wallet.incoming) if wallet else 0
        outgoing_cnt = len(wallet.outgoing) if wallet else 0
        tooltip = (
            f"Adres: {node}\n"
            f"Bakiye: {balance:.4f} BTC\n"
            f"Gelen işlem: {incoming_cnt}\n"
            f"Giden işlem: {outgoing_cnt}"
        )

        net.add_node(
            node,
            label=f"{node}\n{balance:.2f}₿",
            title=tooltip,
            size=size,
            color=node_color,
            font={"size": 9, "color": "white"},
        )

    # Kenar ağırlık sınırlarını hesapla
    weights = [
        d.get("weight", 1.0)
        for _, _, d in network.graph.edges(data=True)
    ]
    min_w = min(weights) if weights else 1.0
    max_w = max(weights) if weights else 1.0

    # ---- Kenarları ekle ----
    for src, dst, data in network.graph.edges(data=True):
        weight = data.get("weight", 1.0)
        norm_w = _normalize(weight, min_w, max_w)

        # Kenar kalınlığını miktara göre ölçekle: 1–9 piksel
        width = 1 + norm_w * 8

        is_highlighted = (src, dst) in h_edges
        edge_color = "#FF6B35" if is_highlighted else "#556677"

        count = data.get("count", 1)
        tooltip_e = (
            f"{src} → {dst}\n"
            f"Toplam: {weight:.4f} BTC\n"
            f"İşlem sayısı: {count}"
        )

        net.add_edge(
            src,
            dst,
            width=width,
            color={"color": edge_color, "highlight": "#FFD700"},
            title=tooltip_e,
            arrows="to",
        )

    # Fizik simülasyonu: Barnes-Hut algoritması
    physics_opts = {
        "physics": {
            "enabled": True,
            "barnesHut": {
                "gravitationalConstant": -9000,
                "centralGravity": 0.25,
                "springLength": 130,
                "springConstant": 0.04,
                "damping": 0.09,
            },
            "stabilization": {"iterations": 150},
        },
        "edges": {
            "smooth": {"type": "curvedCW", "roundness": 0.15}
        },
        "interaction": {
            "hover": True,
            "tooltipDelay": 100,
        },
    }
    net.set_options("var options = " + json.dumps(physics_opts))

    # HTML'i geçici dosya üzerinden oku ve döndür
    with tempfile.NamedTemporaryFile(
        suffix=".html", delete=False, mode="w", encoding="utf-8"
    ) as tmp:
        tmp_path = tmp.name

    net.save_graph(tmp_path)

    with open(tmp_path, "r", encoding="utf-8") as f:
        html_content = f.read()

    os.unlink(tmp_path)
    return html_content


# =============================================================
# STREAMLIT SAYFA FONKSİYONLARI
# =============================================================

def _init_session() -> None:
    """
    Streamlit session state'i ilk çalıştırmada başlatır.
    Yeniden yükleme (rerun) sırasında veri kaybolmaz.
    """
    if "network" not in st.session_state:
        with st.spinner("🔄 Sentetik blokzincir verisi üretiliyor…"):
            net = SyntheticDataGenerator.generate(
                num_wallets=18, num_transactions=45, seed=42
            )
            st.session_state.network = net
            st.session_state.merkle = MerkleTree(net.transactions)
            st.session_state.analyzer = FundFlowAnalyzer(net)
            st.session_state.h_nodes = []
            st.session_state.h_edges = []
            st.session_state.flow_result = None
            st.session_state.last_algo = "BFS"
            st.session_state.last_wallet = ""


def _sidebar(network: BlockchainNetwork, analyzer: FundFlowAnalyzer) -> None:
    """Kenar çubuğu: kontrol paneli, analiz araçları ve istatistikler."""

    st.sidebar.header("⚙️ Kontrol Paneli")

    # ---- Yeni Veri Üretici ----
    if st.sidebar.button("🔄 Yeni Veri Üret", use_container_width=True):
        new_seed = random.randint(0, 99999)
        new_net = SyntheticDataGenerator.generate(
            num_wallets=18, num_transactions=45, seed=new_seed
        )
        st.session_state.network = new_net
        st.session_state.merkle = MerkleTree(new_net.transactions)
        st.session_state.analyzer = FundFlowAnalyzer(new_net)
        st.session_state.h_nodes = []
        st.session_state.h_edges = []
        st.session_state.flow_result = None
        st.session_state.last_wallet = ""
        st.rerun()

    st.sidebar.divider()

    # ---- Fon Akışı Analizi ----
    st.sidebar.subheader("🔍 Fon Akışı Analizi")

    wallet_list = list(network.wallets.keys())
    selected = st.sidebar.selectbox(
        "Başlangıç Cüzdanı:",
        options=[""] + wallet_list,
        format_func=lambda x: x if x else "— Cüzdan seçin —",
        key="sel_wallet",
    )

    algo = st.sidebar.radio(
        "Algoritma:",
        options=["BFS (Genişlik Öncelikli)", "DFS (Derinlik Öncelikli)"],
        index=0,
        key="sel_algo",
    )

    depth = st.sidebar.slider(
        "Maksimum Derinlik:", min_value=1, max_value=6, value=3, key="sel_depth"
    )

    if st.sidebar.button(
        "🚀 Analizi Başlat",
        use_container_width=True,
        disabled=not selected,
    ):
        if "BFS" in algo:
            result = analyzer.bfs_fund_flow(selected, depth)
        else:
            result = analyzer.dfs_fund_flow(selected, depth)

        st.session_state.h_nodes = result["nodes"]
        st.session_state.h_edges = result["edges"]
        st.session_state.flow_result = result
        st.session_state.last_algo = "BFS" if "BFS" in algo else "DFS"
        st.session_state.last_wallet = selected
        st.rerun()

    if st.sidebar.button("❌ Vurgulamayı Temizle", use_container_width=True):
        st.session_state.h_nodes = []
        st.session_state.h_edges = []
        st.session_state.flow_result = None
        st.session_state.last_wallet = ""
        st.rerun()

    st.sidebar.divider()

    # ---- Ağ İstatistikleri ----
    st.sidebar.subheader("📊 Ağ İstatistikleri")
    st.sidebar.metric("Toplam Cüzdan", len(network.wallets))
    st.sidebar.metric("Toplam İşlem", len(network.transactions))
    st.sidebar.metric("Graf Kenar Sayısı", network.graph.number_of_edges())
    st.sidebar.metric("Ort. Bağlantı", network.average_degree())
    st.sidebar.metric("Toplam Hacim", f"{network.total_volume():.2f} BTC")


def _tab_network(network: BlockchainNetwork) -> None:
    """Sekme 1: PyVis interaktif ağ grafiği."""

    flow = st.session_state.flow_result

    # Analiz sonucu varsa özet kartları göster
    if flow:
        c1, c2, c3, c4 = st.columns(4)
        c1.metric("Başlangıç", st.session_state.last_wallet or "—")
        c2.metric("Algoritma", st.session_state.last_algo)
        c3.metric("Ziyaret Edilen Düğüm", len(flow["nodes"]))
        c4.metric("İzlenen Kenar", len(flow["edges"]))

        # Gezinme sırası
        order_str = " → ".join(flow["order"][:10])
        if len(flow["order"]) > 10:
            order_str += f" … (+{len(flow['order']) - 10} daha)"
        st.info(f"**{st.session_state.last_algo} Geçiş Sırası:** {order_str}")

    # Graf HTML'ini oluştur ve Streamlit'e göm
    with st.spinner("Graf oluşturuluyor…"):
        html = build_pyvis_html(
            network,
            highlighted_nodes=st.session_state.h_nodes,
            highlighted_edges=st.session_state.h_edges,
        )

    components.html(html, height=640, scrolling=False)

    # Renk açıklaması
    leg_c1, leg_c2, leg_c3 = st.columns(3)
    leg_c1.markdown("🔵 **Normal düğüm** — standart cüzdan")
    leg_c2.markdown("🟠 **Vurgulanan düğüm** — BFS/DFS yolu")
    leg_c3.markdown("📐 **Boyut** — bakiye miktarına orantılı")


def _tab_merkle(merkle: MerkleTree, network: BlockchainNetwork) -> None:
    """Sekme 2: Merkle ağacı bilgisi ve işlem doğrulama."""

    st.subheader("🌳 Merkle Ağacı")

    col_a, col_b = st.columns([1, 1])

    with col_a:
        st.metric("Yaprak Sayısı (İşlem)", len(merkle.leaves))
        st.metric("Ağaç Derinliği (Seviye)", merkle.depth())
        st.markdown("**Merkle Root:**")
        st.code(merkle.root, language=None)

    with col_b:
        # Ağaç seviye özet tablosu
        st.markdown("**Seviye Özeti:**")
        level_data = []
        for i, lvl in enumerate(merkle.levels):
            if i == 0:
                lname = "Yapraklar (Leaf)"
            elif i == merkle.depth() - 1:
                lname = "Kök (Root)"
            else:
                lname = f"Seviye {i}"
            level_data.append({"Seviye": lname, "Düğüm Sayısı": len(lvl)})
        st.dataframe(
            pd.DataFrame(level_data),
            hide_index=True,
            use_container_width=True,
        )

    st.divider()

    # Seviye seviye hash detayı
    with st.expander("🔎 Seviye Seviye Hash Detayı"):
        for i, lvl in enumerate(merkle.levels):
            label = "Yapraklar" if i == 0 else f"Seviye {i}"
            if i == merkle.depth() - 1:
                label = "Kök"
            st.markdown(f"**{label}** — {len(lvl)} hash")
            for j, h in enumerate(lvl[:4]):
                st.caption(f"  [{j}] {h}")
            if len(lvl) > 4:
                st.caption(f"  … ve {len(lvl) - 4} hash daha")

    st.divider()

    # İşlem doğrulama
    st.subheader("✅ İşlem Doğrulama")
    st.markdown(
        "Bir işlem ID'si seçerek Merkle ağacında kayıtlı olup olmadığını doğrulayın."
    )

    if network.transactions:
        tx_ids = [tx.tx_id for tx in network.transactions]
        chosen = st.selectbox(
            "İşlem ID:",
            options=tx_ids,
            key="verify_tx",
        )
        if st.button("🔍 Doğrula"):
            if merkle.verify_transaction(chosen):
                st.success(
                    f"✅ İşlem doğrulandı! TxID `{chosen}` Merkle ağacında kayıtlı."
                )
            else:
                st.error(
                    f"❌ İşlem doğrulanamadı! TxID `{chosen}` bulunamadı."
                )


def _tab_data(network: BlockchainNetwork) -> None:
    """Sekme 3: Cüzdan bakiyeleri ve işlem geçmişi tabloları."""

    col_left, col_right = st.columns([1, 1])

    # ---- Sol: Cüzdan Bakiyeleri ----
    with col_left:
        st.subheader("💰 Cüzdan Bakiyeleri")

        wallet_rows = [w.summary() for w in network.wallets.values()]
        wdf = (
            pd.DataFrame(wallet_rows)
            .sort_values("Bakiye (BTC)", ascending=False)
            .reset_index(drop=True)
        )
        st.dataframe(wdf, use_container_width=True, hide_index=True, height=400)

        st.subheader("🏆 En Yüksek Bakiyeli 7 Cüzdan")
        top7 = wdf.head(7).set_index("Adres")["Bakiye (BTC)"]
        st.bar_chart(top7, height=220)

    # ---- Sağ: Son İşlemler ----
    with col_right:
        st.subheader("📜 İşlem Geçmişi (Son 30)")

        tx_rows = [
            tx.to_dict()
            for tx in sorted(
                network.transactions, key=lambda t: t.timestamp, reverse=True
            )[:30]
        ]
        txdf = pd.DataFrame(tx_rows)
        st.dataframe(txdf, use_container_width=True, hide_index=True, height=400)

        st.subheader("📈 İşlem İstatistikleri")
        amounts = [tx.amount for tx in network.transactions]
        s1, s2, s3, s4 = st.columns(4)
        s1.metric("Toplam Hacim", f"{sum(amounts):.2f} BTC")
        s2.metric("Ortalama", f"{sum(amounts) / len(amounts):.4f} BTC")
        s3.metric("Maksimum", f"{max(amounts):.4f} BTC")
        s4.metric("Minimum", f"{min(amounts):.4f} BTC")


# =============================================================
# ANA FONKSİYON
# =============================================================

def main() -> None:
    """Streamlit uygulamasının giriş noktası."""

    # Sayfa yapılandırması
    st.set_page_config(
        page_title="Blokzincir İşlem Ağları Analizi",
        page_icon="⛓️",
        layout="wide",
        initial_sidebar_state="expanded",
    )

    # Özel CSS
    st.markdown(
        """
        <style>
        .block-container { padding-top: 1.5rem; }
        div[data-testid="metric-container"] {
            background: #1a1f2e;
            border: 1px solid #2d3550;
            border-radius: 8px;
            padding: 0.6rem 1rem;
        }
        </style>
        """,
        unsafe_allow_html=True,
    )

    # Başlık
    st.markdown(
        "<h1 style='text-align:center; color:#4A90D9;'>"
        "⛓️ Blokzincir İşlem Ağları Analizi"
        "</h1>",
        unsafe_allow_html=True,
    )
    st.markdown(
        "<p style='text-align:center; color:#888; margin-bottom:1.5rem;'>"
        "Veri Yapıları ve Algoritmalar Projesi "
        "• NetworkX • Merkle Ağacı • BFS • DFS"
        "</p>",
        unsafe_allow_html=True,
    )

    # Session state başlat
    _init_session()

    network: BlockchainNetwork = st.session_state.network
    merkle: MerkleTree = st.session_state.merkle
    analyzer: FundFlowAnalyzer = st.session_state.analyzer

    # Kenar çubuğu
    _sidebar(network, analyzer)

    # Sekmeler
    tab1, tab2, tab3 = st.tabs(
        ["🌐 Ağ Grafiği", "🌳 Merkle Ağacı", "📋 Veriler"]
    )

    with tab1:
        _tab_network(network)

    with tab2:
        _tab_merkle(merkle, network)

    with tab3:
        _tab_data(network)


# =============================================================
# UYGULAMA GİRİŞ NOKTASI
# =============================================================
if __name__ == "__main__":
    main()
