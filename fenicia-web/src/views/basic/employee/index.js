import React, { useEffect, useState } from 'react';
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

    useEffect(() => {
        loadEmployees();
    }, [pagination.page, pagination.perPage]);

    const loadEmployees = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await employeeClient.getAll(pagination.page, pagination.perPage);
            console.log('Employees API response:', response);
            
            // API might return array directly or Pagination object
            const employeesList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Employees list:', employeesList);
            setEmployees(employeesList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || employeesList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load employees:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar funcionários.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedEmployee(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (employee) => {
        setSelectedEmployee(employee);
        setModalVisible(true);
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
                setSuccessMessage('Funcionário atualizado com sucesso!');
            } else {
                await employeeClient.create(payload);
                setSuccessMessage('Funcionário criado com sucesso!');
            }
            setModalVisible(false);
            loadEmployees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save employee:', err);
            setError(err.response?.data?.title || 'Falha ao salvar funcionário.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!employeeToDelete) return;

        setDeleting(true);
        try {
            await employeeClient.delete(employeeToDelete.id);
            setSuccessMessage('Funcionário excluído com sucesso!');
            setDeleteModalVisible(false);
            setEmployeeToDelete(null);
            loadEmployees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete employee:', err);
            setError(err.response?.data?.title || 'Falha ao excluir funcionário.');
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
                    <strong>Funcionários</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        Novo
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {loading && (
                        <div className="text-center py-4">
                            <CSpinner color="primary" />
                            <p className="mt-2">Carregando...</p>
                        </div>
                    )}

                    {!loading && employees.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum funcionário cadastrado.</p>
                        </div>
                    )}

                    {!loading && employees.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Nome</CTableHeaderCell>
                                        <CTableHeaderCell>E-mail</CTableHeaderCell>
                                        <CTableHeaderCell>Telefone</CTableHeaderCell>
                                        <CTableHeaderCell>Cargo</CTableHeaderCell>
                                        <CTableHeaderCell>Cidade</CTableHeaderCell>
                                        <CTableHeaderCell>Estado</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {employees.map((employee) => (
                                        <CTableRow key={employee.id}>
                                            <CTableDataCell>{employee.name}</CTableDataCell>
                                            <CTableDataCell>{employee.email}</CTableDataCell>
                                            <CTableDataCell>{formatPhone(employee.phoneNumber)}</CTableDataCell>
                                            <CTableDataCell>{employee.positionName || '-'}</CTableDataCell>
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

            {/* Add/Edit Modal */}
            <EmployeeModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                employee={selectedEmployee}
                loading={saving}
            />

            {/* Delete Confirmation Modal */}
            <CModal 
                visible={deleteModalVisible} 
                onClose={() => setDeleteModalVisible(false)}
            >
                <CModalHeader>
                    <CModalTitle>
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        Confirmar Exclusão
                    </CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <p>
                        Tem certeza que deseja excluir o funcionário <strong>{employeeToDelete?.name}</strong>?
                    </p>
                    <p className="text-danger">
                        Esta ação não pode ser desfeita.
                    </p>
                </CModalBody>
                <CModalFooter>
                    <CButton 
                        color="secondary" 
                        onClick={() => setDeleteModalVisible(false)}
                        disabled={deleting}
                    >
                        Cancelar
                    </CButton>
                    <CButton 
                        color="danger" 
                        onClick={handleDelete}
                        disabled={deleting}
                    >
                        {deleting ? 'Excluindo...' : 'Excluir'}
                    </CButton>
                </CModalFooter>
            </CModal>
        </CContainer>
    );
};

export default EmployeeList;
