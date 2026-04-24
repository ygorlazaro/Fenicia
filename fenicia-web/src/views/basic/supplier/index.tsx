import { cilChart, cilPencil, cilPlus, cilTrash, cilTruck, cilWarning } from '@coreui/icons';
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
import { BasicSupplierClient } from '../../../services/basic/basic-supplier-client';
import { AddSupplierCommand } from "../../../types/basic/supplier/add-supplier-command";
import { SupplierPerformance } from '../../../types/basic/supplier/supplier-performance';
import { UpdateSupplierCommand } from "../../../types/basic/supplier/update-supplier-command";
import RenderAnalyticsTab from './performance';
import SupplierModal from './supplier-modal';

const supplierClient = new BasicSupplierClient();

const Suppliers = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    // Supplier list state
    const [suppliers, setSuppliers] = useState([]);
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
    const [selectedSupplier, setSelectedSupplier] = useState<UpdateSupplierCommand | null>(null);
    const [supplierToDelete, setSupplierToDelete] = useState<UpdateSupplierCommand | null>(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Analytics state
    const [performanceLoading, setPerformanceLoading] = useState(false);
    const [performance, setPerformance] = useState<SupplierPerformance | null>(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const supplierId = searchParams.get('id');
        if (supplierId) {
            loadSupplierForEdit(supplierId);
        }
        loadSuppliers();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadPerformance();
        }
    }, [activeTab, analyticsDays]);


    const loadPerformance = async () => {
        try {
            setPerformanceLoading(true);
            const data = await supplierClient.getPerformance(analyticsDays);
            setPerformance(data);
        } catch (err) {
            console.error('Failed to load supplier performance:', err);
            setError(t('suppliers.performanceLoadError'));
        } finally {
            setPerformanceLoading(false);
        }
    };

    const loadSupplierForEdit = async (supplierId: string) => {
        try {
            const supplier = await supplierClient.getById(supplierId);
            setSelectedSupplier(supplier);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load supplier for edit:', err);
            setError(t('suppliers.loadError'));
        }
    };

    const loadSuppliers = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await supplierClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const suppliersList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? suppliersList.length;
            setSuppliers(suppliersList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            console.error('Failed to load suppliers:', err);
            setError(t('suppliers.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedSupplier(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (supplier: UpdateSupplierCommand) => {
        try {
            const fullSupplier = await supplierClient.getById(supplier.id);
            setSelectedSupplier(fullSupplier);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load supplier details:', err);
            setError(t('suppliers.loadError'));
        }
    };

    const handleOpenDelete = (supplier: UpdateSupplierCommand) => {
        setSupplierToDelete(supplier);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData: UpdateSupplierCommand | AddSupplierCommand) => {
        setSaving(true);
        setError(null);

        if (!formData.name || !formData.email) {
            setError(t('suppliers.requiredFields'));
            setSaving(false);
            return;
        }

        try {
            const payload: UpdateSupplierCommand = {
                id: selectedSupplier?.id || crypto.randomUUID(),
                name: formData.name,
                email: formData.email,
                document: formData.document || null,
                phoneNumber: formData.phoneNumber || null,
                address: formData.address || null,
            };

            if (selectedSupplier) {
                await supplierClient.update(selectedSupplier.id, payload);
                setSuccessMessage(t('suppliers.updateSuccess'));
            } else {
                await supplierClient.create(payload as AddSupplierCommand);
                setSuccessMessage(t('suppliers.createSuccess'));
            }
            setModalVisible(false);
            loadSuppliers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save supplier:', err);
            setError(err.response?.data?.title || t('suppliers.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!supplierToDelete) return;

        setDeleting(true);
        try {
            await supplierClient.delete(supplierToDelete.id);
            setSuccessMessage(t('suppliers.deleteSuccess'));
            setDeleteModalVisible(false);
            setSupplierToDelete(null);
            loadSuppliers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete supplier:', err);
            setError(t('suppliers.loadError'));
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

    const formatPhone = (phone) => {
        if (!phone) return '-';
        const cleaned = phone.replace(/\D/g, '');
        if (cleaned.length === 10) {
            return `(${cleaned.slice(0, 2)}) ${cleaned.slice(2, 6)}-${cleaned.slice(6)}`;
        }
        return phone;
    };

    const formatCurrency = (value) => {
        if (!value && value !== 0) return '-';
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
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
                    <strong>{t('suppliers.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('suppliers.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilTruck} className="me-2" />
                                {t('suppliers.suppliersList')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t('suppliers.performance')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Suppliers List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t('common.loading')}</p>
                                </div>
                            )}

                            {!loading && suppliers.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t('common.noData')}</p>
                                </div>
                            )}

                            {!loading && suppliers.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('suppliers.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('suppliers.email')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('suppliers.phone')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('suppliers.document')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {suppliers.map((supplier) => (
                                                <CTableRow key={supplier.id}>
                                                    <CTableDataCell>{supplier.name}</CTableDataCell>
                                                    <CTableDataCell>{supplier.email}</CTableDataCell>
                                                    <CTableDataCell>{formatPhone(supplier.phoneNumber)}</CTableDataCell>
                                                    <CTableDataCell>{supplier.document || '-'}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <CButton
                                                            color="info"
                                                            size="sm"
                                                            className="me-2"
                                                            onClick={() => handleOpenEdit(supplier)}
                                                        >
                                                            <CIcon icon={cilPencil} />
                                                        </CButton>
                                                        <CButton
                                                            color="danger"
                                                            size="sm"
                                                            onClick={() => handleOpenDelete(supplier)}
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

                        {/* Analytics Tab */}
                        <CTabPane visible={activeTab === 1}>
                            <RenderAnalyticsTab performance={performance}
                                performanceLoading={performanceLoading}
                                analyticsDays={analyticsDays}
                                setAnalyticsDays={setAnalyticsDays}
                            />
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

            <SupplierModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                supplier={selectedSupplier}
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
                        {t('suppliers.deleteConfirm', { name: supplierToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('suppliers.deleteWarning')}
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

export default Suppliers;
