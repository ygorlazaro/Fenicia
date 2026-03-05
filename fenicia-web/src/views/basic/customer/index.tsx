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
import CustomerModal from '../../../components/CustomerModal';
import { BasicCustomerClient } from '../../../services/basic-crud-clients';
import Pagination from '../../../components/Pagination';

const customerClient = new BasicCustomerClient();

const Customers = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
    const [customers, setCustomers] = useState([]);
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
    const [selectedCustomer, setSelectedCustomer] = useState(null);
    const [customerToDelete, setCustomerToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const customerId = searchParams.get('id');
        if (customerId) {
            loadCustomerForEdit(customerId);
        }
        loadCustomers();
    }, [pagination.page, pagination.perPage]);

    const loadCustomerForEdit = async (customerId) => {
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

    const handleOpenEdit = async (customer) => {
        try {
            const fullCustomer = await customerClient.getById(customer.id);
            setSelectedCustomer(fullCustomer);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load customer details:', err);
            setError(t('customers.loadError'));
        }
    };

    const handleOpenDelete = (customer) => {
        setCustomerToDelete(customer);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        setError(null);

        if (!formData.name || !formData.email) {
            setError(t('customers.requiredFields'));
            setSaving(false);
            return;
        }

        try {
            const payload = {
                id: selectedCustomer?.id || crypto.randomUUID(),
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

            if (selectedCustomer) {
                await customerClient.update(selectedCustomer.id, payload);
                setSuccessMessage(t('customers.updateSuccess'));
            } else {
                await customerClient.create(payload);
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
                    <strong>{t('customers.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('customers.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
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
