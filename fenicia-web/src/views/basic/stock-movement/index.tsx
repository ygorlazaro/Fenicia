import {
    CAlert,
    CContainer,
    CSpinner
} from '@coreui/react';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import TimeRangeSelector from '../../../components/fenicia/time-range-selector';
import { BasicOrderClient } from '../../../services/basic/basic-order-client';
import { BasicProductClient } from '../../../services/basic/basic-product-client';
import { BasicStockMovementClient } from '../../../services/basic/basic-stock-movement-client';
import { StockMovementDashboard } from '../../../types/basic/stock-movement/stock-movement-dashboard';
import ProductModal from '../product/product-modal';
import MonthlyIdOutChart from './monthly-id-out-chart';
import StockMovementHistoryTable from './stock-movement-history-table';
import SummaryCards from './summary-cards';

const stockMovementClient = new BasicStockMovementClient();

const StockMovementDashboardView = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [dashboard, setDashboard] = useState<StockMovementDashboard | null>(null);
    const [days, setDays] = useState(30);
    
    // Modal state for quick view without navigation
    const [productModalVisible, setProductModalVisible] = useState(false)
    const [selectedItem, setSelectedItem] = useState(null)
    const [modalLoading, setModalLoading] = useState(false)
    
    const productClient = new BasicProductClient()
    const orderClient = new BasicOrderClient()

    useEffect(() => {
        loadDashboard();
    }, [days]);

    const loadDashboard = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await stockMovementClient.getDashboard(days);
            setDashboard(data);
        } catch (err) {
            setError(t('stockMovement.loadError'));
            console.error('Failed to load stock movement dashboard:', err);
        } finally {
            setLoading(false);
        }
    };

    // Open modal without navigation
    const openProductModal = async (productId: string, e: React.MouseEvent) => {
        e.preventDefault()
        e.stopPropagation()
        try {
            setModalLoading(true)
            const product = await productClient.getById(productId)
            setSelectedItem(product)
            setProductModalVisible(true)
        } catch (err) {
            console.error('Failed to load product:', err)
            navigate(`/basic/products?id=${productId}`)
        } finally {
            setModalLoading(false)
        }
    }

    const openOrderModal = async (orderId: string, e: React.MouseEvent) => {
        e.preventDefault()
        e.stopPropagation()
        try {
            setModalLoading(true)
            const order = await orderClient.getById(orderId)
            setSelectedItem(order)
        } catch (err) {
            console.error('Failed to load order:', err)
            navigate(`/basic/order/${orderId}`)
        } finally {
            setModalLoading(false)
        }
    }

    if (loading) {
        return (
            <CContainer className="py-4">
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            </CContainer>
        );
    }

    if (error) {
        return (
            <CContainer className="py-4">
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error}
                </CAlert>
            </CContainer>
        );
    }

    return (
        <CContainer className="py-4">
            <TimeRangeSelector days={days} setDays={setDays} />
            <SummaryCards dashboard={dashboard} />
            <MonthlyIdOutChart monthlyInOut={dashboard?.monthlyInOut} />
            <StockMovementHistoryTable history={dashboard?.history} openProductModal={openProductModal} openOrderModal={openOrderModal} />

            {/* Top Moved Products and Turnover Rates */}


            {/* Quick View Modals */}
            <ProductModal
                visible={productModalVisible}
                onClose={() => {
                    setProductModalVisible(false)
                    setSelectedItem(null)
                }}
                onSave={() => {
                    setProductModalVisible(false)
                    setSelectedItem(null)
                    loadDashboard()
                }}
                product={selectedItem}
                loading={modalLoading}
            />
        </CContainer>
    );
};

export default StockMovementDashboardView;
