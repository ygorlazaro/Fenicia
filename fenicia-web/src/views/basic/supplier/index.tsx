import React, { useEffect, useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useSearchParams } from 'react-router-dom';
import {
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CContainer,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CSpinner,
    CAlert
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning } from '@coreui/icons';
import SupplierModal from '../../../components/SupplierModal';
import { BasicSupplierClient } from '../../../services/basic-crud-clients';
import Pagination from '../../../components/Pagination';

const supplierClient = new BasicSupplierClient();

const Suppliers = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
    const [suppliers, setSuppliers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [pagination, setPagination] = useState({
        page: 1,
        perPage: 10,
        total: 0,
        pages: 0
    });
    const [modalVisible, setModalVisible] = useState(false);
    const [deleteModalVisible, setDeleteModalVisible] = useState(false);
    const [selectedSupplier, setSelectedSupplier] = useState(null);
    const [supplierToDelete, setSupplierToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const supplierId = searchParams.get('id');
        if (supplierId) {
            loadSupplierForEdit(supplierId);
        }
        loadSuppliers();
    }, [pagination.page, pagination.perPage]);

    const loadSupplierForEdit = async (supplierId) => {
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

    const handleOpenEdit = async (supplier) => {
        try {
            const fullSupplier = await supplierClient.getById(supplier.id);
            setSelectedSupplier(fullSupplier);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load supplier details:', err);
            setError(t('suppliers.loadError'));
        }
    };

    const handleOpenDelete = (supplier) => {
        setSupplierToDelete(supplier);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        setError(null);

        if (!formData.name || !formData.email) {
            setError(t('suppliers.requiredFields'));
            setSaving(false);
            return;
        }

        try {
            const payload = {
                id: selectedSupplier?.id || crypto.randomUUID(),
                name: formData.name,
                email: formData.email,
                document: formData.document || null,
                phoneNumber: formData.phoneNumber || null,
                street: formData.street || null,
                number: formData.number || null,
                neighborhood: formData.neighborhood || null,
                city: formData.city || null,
                complement: formData.complement || null,
                zipCode: formData.zipCode || null,
                stateId: formData.stateId || null
            };

            if (selectedSupplier) {
                await supplierClient.update(selectedSupplier.id, payload);
                setSuccessMessage(t('suppliers.updateSuccess'));
            } else {
                await supplierClient.create(payload);
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
