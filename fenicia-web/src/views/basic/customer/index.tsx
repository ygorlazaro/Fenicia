import { cilChart, cilPencil, cilPeople, cilPlus, cilTrash, cilWarning } from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
    CAlert,
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CContainer,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CNav,
    CNavItem,
    CNavLink,
    CSpinner,
    CTabContent,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CTabPane
} from '@coreui/react';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useSearchParams } from 'react-router-dom';
import Pagination from '../../../components/fenicia/pagination';
import BasicCustomerClient from '../../../services/basic/basic-customer-client';
import { AddCustomerCommand } from "../../../types/basic/customer/add-customer-command";
import { CustomerInsights } from '../../../types/basic/customer/customer-insights';
import { GetAllCustomerResponse } from "../../../types/basic/customer/get-all-customer-response";
import { UpdateCustomerCommand } from "../../../types/basic/customer/update-customer-command";
import formatPhone from '../../../utils/format-phone';
import CustomerModal from './customer-model';
import { RenderAnalyticsTab } from './performance';

const customerClient = new BasicCustomerClient();
const insightsClient = new BasicCustomerClient();

const Customers = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    // Customer list state
    const [customers, setCustomers] = useState<GetAllCustomerResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [pagination, setPagination] = useState({
        page: 1,
        perPage: 10,
        total: 0,
        pages: 0
    });

    // Modal state
    const [modalVisible, setModalVisible] = useState(false);
    const [deleteModalVisible, setDeleteModalVisible] = useState(false);
    const [selectedCustomer, setSelectedCustomer] = useState(null);
    const [customerToDelete, setCustomerToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Analytics state
    const [insightsLoading, setInsightsLoading] = useState(false);
    const [insights, setInsights] = useState<CustomerInsights | null>(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const customerId = searchParams.get('id');
        if (customerId) {
            loadCustomerForEdit(customerId);
        }
        loadCustomers();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadInsights();
        }
    }, [activeTab, analyticsDays]);

    const loadInsights = async () => {
        try {
            setInsightsLoading(true);
            const data = await insightsClient.getInsights(analyticsDays);
            setInsights(data);
        } catch (err) {
            console.error('Failed to load customer insights:', err);
            setError(t('customers.insightsLoadError'));
        } finally {
            setInsightsLoading(false);
        }
    };

    const loadCustomerForEdit = async (customerId: string) => {
        try {
            const customer = await customerClient.getById(customerId);
            setSelectedCustomer(customer);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load customer for edit:', err);
            setError(t('customers.loadError'));
        }
    };

    const loadCustomers = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await customerClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const customersList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? customersList.length;
            setCustomers(customersList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            console.error('Failed to load customers:', err);
            setError(t('customers.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedCustomer(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (customer: AddCustomerCommand) => {
        try {
            const fullCustomer = await customerClient.getById(customer.id);
            setSelectedCustomer(fullCustomer);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load customer details:', err);
            setError(t('customers.loadError'));
        }
    };

    const handleOpenDelete = (customer: UpdateCustomerCommand) => {
        setCustomerToDelete(customer);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData: AddCustomerCommand | UpdateCustomerCommand) => {
        setSaving(true);
        setError(null);

        if (!formData.name || !formData.email) {
            setError(t('customers.requiredFields'));
            setSaving(false);
            return;
        }

        try {
            const payload: UpdateCustomerCommand = {
                id: selectedCustomer?.id || crypto.randomUUID(),
                name: formData.name,
                email: formData.email,
                document: formData.document || null,
                phoneNumber: formData.phoneNumber || null,
                address: formData.address
            };

            if (selectedCustomer) {
                await customerClient.update(selectedCustomer.id, payload);
                setSuccessMessage(t('customers.updateSuccess'));
            } else {
                await customerClient.create(payload as AddCustomerCommand);
                setSuccessMessage(t('customers.createSuccess'));
            }
            setModalVisible(false);
            loadCustomers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save customer:', err);
            setError(err.response?.data?.title || t('customers.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!customerToDelete) return;

        setDeleting(true);
        try {
            await customerClient.delete(customerToDelete.id);
            setSuccessMessage(t('customers.deleteSuccess'));
            setDeleteModalVisible(false);
            setCustomerToDelete(null);
            loadCustomers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete customer:', err);
            setError(t('customers.loadError'));
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (newPage) => {
        setPagination(prev => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage) => {
        setPagination(prev => ({ ...prev, perPage: newPerPage, page: 1 }));
    };

    return (
        <CContainer className="py-4">
            {error && (
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error}
                </CAlert>
            )}

            {successMessage && (
                <CAlert color="success" dismissible onClose={() => setSuccessMessage(null)}>
                    {successMessage}
                </CAlert>
            )}

            <CCard>
                <CCardHeader className="d-flex justify-content-between align-items-center">
                    <strong>{t('customers.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('customers.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilPeople} className="me-2" />
                                {t('customers.customersList')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t('customers.insights')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Customers List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t('common.loading')}</p>
                                </div>
                            )}

                            {!loading && customers.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t('common.noData')}</p>
                                </div>
                            )}

                            {!loading && customers.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('customers.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.email')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.phone')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.document')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {customers.map((customer) => (
                                                <CTableRow key={customer.id}>
                                                    <CTableDataCell>{customer.name}</CTableDataCell>
                                                    <CTableDataCell>{customer.email}</CTableDataCell>
                                                    <CTableDataCell>{formatPhone(customer.phoneNumber)}</CTableDataCell>
                                                    <CTableDataCell>{customer.document || '-'}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <CButton
                                                            color="info"
                                                            size="sm"
                                                            className="me-2"
                                                            onClick={() => handleOpenEdit(customer)}
                                                        >
                                                            <CIcon icon={cilPencil} />
                                                        </CButton>
                                                        <CButton
                                                            color="danger"
                                                            size="sm"
                                                            onClick={() => handleOpenDelete(customer)}
                                                        >
                                                            <CIcon icon={cilTrash} />
                                                        </CButton>
                                                    </CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>

                                    <Pagination
                                        pagination={pagination}
                                        onPageChange={handlePageChange}
                                        onPerPageChange={handlePerPageChange}
                                    />
                                </>
                            )}
                        </CTabPane>

                        <CTabPane visible={activeTab === 1}>
                            <RenderAnalyticsTab insightsLoading={insightsLoading} insights={insights} analyticsDays={analyticsDays} setAnalyticsDays={setAnalyticsDays} />
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

            <CustomerModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                customer={selectedCustomer}
                loading={saving}
            />

            <CModal
                visible={deleteModalVisible}
                onClose={() => setDeleteModalVisible(false)}
            >
                <CModalHeader>
                    <CModalTitle>
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        {t('common.confirmDelete')}
                    </CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <p>
                        {t('customers.deleteConfirm', { name: customerToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('customers.deleteWarning')}
                    </p>
                </CModalBody>
                <CModalFooter>
                    <CButton
                        color="secondary"
                        onClick={() => setDeleteModalVisible(false)}
                        disabled={deleting}
                    >
                        {t('common.cancel')}
                    </CButton>
                    <CButton
                        color="danger"
                        onClick={handleDelete}
                        disabled={deleting}
                    >
                        {deleting ? t('common.deleting') : t('common.delete')}
                    </CButton>
                </CModalFooter>
            </CModal>
        </CContainer>
    );
};

export default Customers;
