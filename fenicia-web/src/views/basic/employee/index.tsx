import React, { useEffect, useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useSearchParams, Link } from 'react-router-dom';
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
import EmployeeModal from '../../../components/EmployeeModal';
import BasicEmployeeClient from "src/services/basic-employee-client";
import Pagination from '../../../components/Pagination';

const employeeClient = new BasicEmployeeClient("http://localhost:5083");

const EmployeeList = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
    const [employees, setEmployees] = useState([]);
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
    const [selectedEmployee, setSelectedEmployee] = useState(null);
    const [employeeToDelete, setEmployeeToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const employeeId = searchParams.get('id');
        if (employeeId) {
            loadEmployeeForEdit(employeeId);
        }
        loadEmployees();
    }, [pagination.page, pagination.perPage]);

    const loadEmployeeForEdit = async (employeeId) => {
        try {
            const employee = await employeeClient.getById(employeeId);
            setSelectedEmployee(employee);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load employee for edit:', err);
            setError(t('employees.loadError'));
        }
    };

    const loadEmployees = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await employeeClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const employeesList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? employeesList.length;
            setEmployees(employeesList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            setError(t('employees.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedEmployee(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (employee) => {
        try {
            const fullEmployee = await employeeClient.getById(employee.id);
            setSelectedEmployee(fullEmployee);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load employee details:', err);
            setError(t('employees.loadError'));
        }
    };

    const handleOpenDelete = (employee) => {
        setEmployeeToDelete(employee);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            const payload = {
                id: formData.id || crypto.randomUUID(),
                positionId: formData.positionId,
                name: formData.name,
                email: formData.email,
                document: formData.document || null,
                city: formData.city || null,
                complement: formData.complement || null,
                neighborhood: formData.neighborhood || null,
                number: formData.number || null,
                stateId: formData.stateId,
                street: formData.street || null,
                zipCode: formData.zipCode || null,
                phoneNumber: formData.phoneNumber || null
            };

            if (selectedEmployee) {
                await employeeClient.update(selectedEmployee.id, payload);
                setSuccessMessage(t('employees.updateSuccess'));
            } else {
                await employeeClient.create(payload);
                setSuccessMessage(t('employees.createSuccess'));
            }
            setModalVisible(false);
            loadEmployees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('employees.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!employeeToDelete) return;

        setDeleting(true);
        try {
            await employeeClient.delete(employeeToDelete.id);
            setSuccessMessage(t('employees.deleteSuccess'));
            setDeleteModalVisible(false);
            setEmployeeToDelete(null);
            loadEmployees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('employees.loadError'));
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

    const formatPhone = (phone: string) => {
        if (!phone) return '-';
        const cleaned = phone.replace(/\D/g, '');
        if (cleaned.length === 10) {
            return `(${cleaned.slice(0,2)}) ${cleaned.slice(2,6)}-${cleaned.slice(6)}`;
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
                    <strong>{t('employees.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('employees.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {loading && (
                        <div className="text-center py-4">
                            <CSpinner color="primary" />
                            <p className="mt-2">{t('common.loading')}</p>
                        </div>
                    )}

                    {!loading && employees.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">{t('common.noData')}</p>
                        </div>
                    )}

                    {!loading && employees.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t('employees.name')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('employees.email')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('employees.phone')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('employees.position')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('employees.city')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('employees.state')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {employees.map((employee) => (
                                        <CTableRow key={employee.id}>
                                            <CTableDataCell>{employee.name}</CTableDataCell>
                                            <CTableDataCell>{employee.email}</CTableDataCell>
                                            <CTableDataCell>{formatPhone(employee.phoneNumber)}</CTableDataCell>
                                            <CTableDataCell>
                                                {employee.positionId ? (
                                                    <Link to={`/basic/positions?id=${employee.positionId}`} className="text-decoration-none">
                                                        {employee.positionName || '-'}
                                                    </Link>
                                                ) : (
                                                    '-'
                                                )}
                                            </CTableDataCell>
                                            <CTableDataCell>{employee.city || '-'}</CTableDataCell>
                                            <CTableDataCell>{employee.stateName || '-'}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton 
                                                    color="info" 
                                                    size="sm" 
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(employee)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton 
                                                    color="danger" 
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(employee)}
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

            <EmployeeModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                employee={selectedEmployee}
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
                        {t('employees.deleteConfirm', { name: employeeToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('employees.deleteWarning')}
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

export default EmployeeList;
