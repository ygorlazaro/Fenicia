import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {
    CCard,
    CCardBody,
    CCardHeader,
    CContainer,
    CSpinner,
    CAlert,
    CRow,
    CCol,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CButton,
    CBadge
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilArrowLeft, cilPrint, cilCart, cilCalendar, cilUser, cilDollar } from '@coreui/icons';
import { BasicOrderClient } from '../../../services/basic-crud-clients';

const orderClient = new BasicOrderClient();

interface OrderDetail {
    productId: string;
    productName: string;
    quantity: number;
    price: number;
    subtotal: number;
}

interface Order {
    id: string;
    customerId: string;
    customerName: string;
    totalAmount: number;
    saleDate: string;
    status: string;
    totalItems: number;
    employeeId?: string;
    employeeName?: string;
}

const OrderDetailPage = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { t } = useTranslation();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [order, setOrder] = useState<Order | null>(null);
    const [details, setDetails] = useState<OrderDetail[]>([]);

    useEffect(() => {
        loadOrderDetails();
    }, [id]);

    const loadOrderDetails = async () => {
        if (!id) return;
        
        try {
            setLoading(true);
            setError(null);
            
            const [orderData, detailsData] = await Promise.all([
                orderClient.getById(id),
                orderClient.getDetails(id)
            ]);
            
            setOrder(orderData);
            setDetails(detailsData);
        } catch (err) {
            setError(t('orders.loadError'));
            console.error('Failed to load order details:', err);
        } finally {
            setLoading(false);
        }
    };

    const handlePrint = () => {
        window.print();
    };

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
        }).format(value);
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString();
    };

    const getStatusBadgeColor = (status: string): string => {
        switch (status?.toLowerCase()) {
            case 'pending':
                return 'warning';
            case 'approved':
                return 'success';
            case 'cancelled':
                return 'danger';
            default:
                return 'secondary';
        }
    };

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

    if (error || !order) {
        return (
            <CContainer className="py-4">
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error || t('common.noData')}
                </CAlert>
                <CButton color="primary" onClick={() => navigate('/basic/orders')}>
                    <CIcon icon={cilArrowLeft} className="me-2" />
                    {t('common.back')}
                </CButton>
            </CContainer>
        );
    }

    return (
        <CContainer className="py-4">
            {/* Print Styles */}
            <style>{`
                @media print {
                    .no-print {
                        display: none !important;
                    }
                    .card {
                        border: none !important;
                        box-shadow: none !important;
                    }
                    body {
                        background: white !important;
                    }
                }
            `}</style>

            {/* Header Actions */}
            <div className="d-flex justify-content-between align-items-center mb-4 no-print">
                <CButton color="primary" onClick={() => navigate('/basic/orders')}>
                    <CIcon icon={cilArrowLeft} className="me-2" />
                    {t('common.back')}
                </CButton>
                <CButton color="secondary" onClick={handlePrint}>
                    <CIcon icon={cilPrint} className="me-2" />
                    {t('common.print')}
                </CButton>
            </div>

            {/* Order Header */}
            <CCard className="mb-4">
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilCart} className="me-2" size="lg" />
                    <strong>{t('orders.orderDetails')} #{order.id.substring(0, 8)}</strong>
                </CCardHeader>
                <CCardBody>
                    <CRow xs={{ gutter: 3 }}>
                        <CCol md={4}>
                            <div className="text-muted small">{t('orders.customer')}</div>
                            <Link to={`/basic/customers?id=${order.customerId}`} className="text-decoration-none">
                                <div className="d-flex align-items-center">
                                    <CIcon icon={cilUser} className="me-2 text-primary" />
                                    <strong>{order.customerName}</strong>
                                </div>
                            </Link>
                        </CCol>
                        <CCol md={4}>
                            <div className="text-muted small">{t('orders.date')}</div>
                            <div className="d-flex align-items-center">
                                <CIcon icon={cilCalendar} className="me-2 text-primary" />
                                <strong>{formatDate(order.saleDate)}</strong>
                            </div>
                        </CCol>
                        <CCol md={4}>
                            <div className="text-muted small">{t('orders.statusLabel')}</div>
                            <CBadge color={getStatusBadgeColor(order.status)} size="lg">
                                {t(`orders.statusValues.${order.status.toLowerCase()}`)}
                            </CBadge>
                        </CCol>
                        {order.employeeName && (
                            <CCol md={6}>
                                <div className="text-muted small">{t('orders.employee')}</div>
                                <Link to={`/basic/employees?id=${order.employeeId}`} className="text-decoration-none">
                                    <div className="d-flex align-items-center">
                                        <CIcon icon={cilUser} className="me-2 text-info" />
                                        <span>{order.employeeName}</span>
                                    </div>
                                </Link>
                            </CCol>
                        )}
                        <CCol md={6}>
                            <div className="text-muted small">{t('orders.totalItems')}</div>
                            <div><strong>{order.totalItems} {t('orders.items')}</strong></div>
                        </CCol>
                        <CCol md={12} className="border-top pt-3 mt-2">
                            <div className="d-flex justify-content-between align-items-center">
                                <div className="d-flex align-items-center">
                                    <CIcon icon={cilDollar} className="me-2 text-success" size="lg" />
                                    <span className="text-muted">{t('orders.totalAmount')}</span>
                                </div>
                                <div className="text-success">
                                    <strong className="fs-3">{formatCurrency(order.totalAmount)}</strong>
                                </div>
                            </div>
                        </CCol>
                    </CRow>
                </CCardBody>
            </CCard>

            {/* Order Items */}
            <CCard className="mb-4">
                <CCardHeader>
                    <strong>{t('orders.items')}</strong>
                </CCardHeader>
                <CCardBody>
                    {details.length === 0 ? (
                        <p className="text-muted text-center">{t('common.noData')}</p>
                    ) : (
                        <CTable hover responsive>
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>#</CTableHeaderCell>
                                    <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('orders.quantity')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">{t('products.price')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">{t('orders.subtotal')}</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {details.map((item, index) => (
                                    <CTableRow key={item.productId}>
                                        <CTableDataCell>{index + 1}</CTableDataCell>
                                        <CTableDataCell>
                                            <Link to={`/basic/products?id=${item.productId}`} className="text-decoration-none">
                                                <strong>{item.productName}</strong>
                                            </Link>
                                        </CTableDataCell>
                                        <CTableDataCell className="text-center">
                                            {item.quantity}
                                        </CTableDataCell>
                                        <CTableDataCell className="text-end">
                                            {formatCurrency(item.price)}
                                        </CTableDataCell>
                                        <CTableDataCell className="text-end">
                                            <strong>{formatCurrency(item.subtotal)}</strong>
                                        </CTableDataCell>
                                    </CTableRow>
                                ))}
                            </CTableBody>
                            <CTableBody>
                                <CTableRow>
                                    <CTableDataCell colSpan={4} className="text-end fw-bold">
                                        {t('orders.total')}:
                                    </CTableDataCell>
                                    <CTableDataCell className="text-end fw-bold text-success">
                                        {formatCurrency(order.totalAmount)}
                                    </CTableDataCell>
                                </CTableRow>
                            </CTableBody>
                        </CTable>
                    )}
                </CCardBody>
            </CCard>

            {/* Order Information */}
            <CCard>
                <CCardHeader>
                    <strong>{t('orders.information')}</strong>
                </CCardHeader>
                <CCardBody>
                    <CRow xs={{ gutter: 3 }}>
                        <CCol md={6}>
                            <p className="mb-1 text-muted small">Order ID</p>
                            <Link to={`/basic/order/${order.id}`} className="font-monospace text-decoration-none">
                                {order.id}
                            </Link>
                        </CCol>
                        <CCol md={6}>
                            <p className="mb-1 text-muted small">{t('orders.customer')} ID</p>
                            <Link to={`/basic/customers?id=${order.customerId}`} className="font-monospace text-decoration-none">
                                {order.customerId}
                            </Link>
                        </CCol>
                        {order.employeeId && (
                            <CCol md={6}>
                                <p className="mb-1 text-muted small">{t('orders.employee')} ID</p>
                                <Link to={`/basic/employees?id=${order.employeeId}`} className="font-monospace text-decoration-none">
                                    {order.employeeId}
                                </Link>
                            </CCol>
                        )}
                    </CRow>
                </CCardBody>
            </CCard>
        </CContainer>
    );
};

export default OrderDetailPage;
