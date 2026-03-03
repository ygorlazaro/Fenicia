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
    CAlert,
    CPagination,
    CPaginationItem,
    CBadge
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning } from '@coreui/icons';
import ProjectTaskAssigneeClient from '../../../services/project-task-assignee-client';
import ProjectTaskAssigneeModal from '../../../components/ProjectTaskAssigneeModal';

const projectTaskAssigneeClient = new ProjectTaskAssigneeClient("http://localhost:5144");

const ProjectTaskAssigneeList = () => {
    const [assignees, setAssignees] = useState([]);
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
    const [selectedAssignee, setSelectedAssignee] = useState(null);
    const [assigneeToDelete, setAssigneeToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadAssignees();
    }, [pagination.page, pagination.perPage]);

    const loadAssignees = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await projectTaskAssigneeClient.getAll(pagination.page, pagination.perPage);
            console.log('Project Task Assignees API response:', response);

            const assigneesList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Project Task Assignees list:', assigneesList);
            setAssignees(assigneesList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || assigneesList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load project task assignees:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar responsáveis das tarefas do projeto.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedAssignee(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (assignee) => {
        setSelectedAssignee(assignee);
        setModalVisible(true);
    };

    const handleOpenDelete = (assignee) => {
        setAssigneeToDelete(assignee);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedAssignee) {
                await projectTaskAssigneeClient.update(selectedAssignee.id, formData);
                setSuccessMessage('Responsável da tarefa atualizado com sucesso!');
            } else {
                await projectTaskAssigneeClient.create(formData);
                setSuccessMessage('Responsável da tarefa criado com sucesso!');
            }
            setModalVisible(false);
            loadAssignees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save project task assignee:', err);
            setError(err.response?.data?.title || 'Falha ao salvar responsável da tarefa do projeto.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!assigneeToDelete) return;

        setDeleting(true);
        try {
            await projectTaskAssigneeClient.delete(assigneeToDelete.id);
            setSuccessMessage('Responsável da tarefa excluído com sucesso!');
            setDeleteModalVisible(false);
            setAssigneeToDelete(null);
            loadAssignees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete project task assignee:', err);
            setError(err.response?.data?.title || 'Falha ao excluir responsável da tarefa do projeto.');
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (page) => {
        setPagination(prev => ({ ...prev, page }));
    };

    const getRoleBadgeColor = (role) => {
        const colorMap = {
            'Owner': 'danger',
            'Contributor': 'primary',
            'Reviewer': 'info'
        };
        return colorMap[role] || 'secondary';
    };

    const translateRole = (role) => {
        const translations = {
            'Owner': 'Proprietário',
            'Contributor': 'Contribuidor',
            'Reviewer': 'Revisor'
        };
        return translations[role] || role;
    };

    const formatDate = (dateString) => {
        if (!dateString) return '-';
        return new Date(dateString).toLocaleDateString('pt-BR');
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
                    <strong>Responsáveis das Tarefas</strong>
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

                    {!loading && assignees.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum responsável de tarefa cadastrado.</p>
                        </div>
                    )}

                    {!loading && assignees.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>ID da Tarefa</CTableHeaderCell>
                                        <CTableHeaderCell>ID do Usuário</CTableHeaderCell>
                                        <CTableHeaderCell>Função</CTableHeaderCell>
                                        <CTableHeaderCell>Atribuído Em</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {assignees.map((assignee) => (
                                        <CTableRow key={assignee.id}>
                                            <CTableDataCell>{assignee.taskId || '-'}</CTableDataCell>
                                            <CTableDataCell>{assignee.userId || '-'}</CTableDataCell>
                                            <CTableDataCell>
                                                <CBadge color={getRoleBadgeColor(assignee.role)}>
                                                    {translateRole(assignee.role)}
                                                </CBadge>
                                            </CTableDataCell>
                                            <CTableDataCell>{formatDate(assignee.assignedAt)}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(assignee)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(assignee)}
                                                >
                                                    <CIcon icon={cilTrash} />
                                                </CButton>
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>

                            <div className="d-flex justify-content-between align-items-center mt-3">
                                <small className="text-muted">
                                    Mostrando {assignees.length} de {pagination.total} registro(s)
                                </small>
                                <CPagination>
                                    <CPaginationItem
                                        onClick={() => handlePageChange(pagination.page - 1)}
                                        disabled={pagination.page === 1}
                                    >
                                        Anterior
                                    </CPaginationItem>
                                    {Array.from({ length: Math.min(5, pagination.pages) }, (_, i) => {
                                        let pageNum = i + 1;
                                        if (pagination.pages > 5) {
                                            if (pagination.page > 3) {
                                                pageNum = pagination.page - 2 + i;
                                            }
                                            if (pageNum > pagination.pages) {
                                                pageNum = pagination.pages - 4 + i;
                                            }
                                        }
                                        return (
                                            <CPaginationItem
                                                key={pageNum}
                                                active={pageNum === pagination.page}
                                                onClick={() => handlePageChange(pageNum)}
                                            >
                                                {pageNum}
                                            </CPaginationItem>
                                        );
                                    })}
                                    <CPaginationItem
                                        onClick={() => handlePageChange(pagination.page + 1)}
                                        disabled={pagination.page === pagination.pages}
                                    >
                                        Próximo
                                    </CPaginationItem>
                                </CPagination>
                            </div>
                        </>
                    )}
                </CCardBody>
            </CCard>

            {/* Add/Edit Modal */}
            <ProjectTaskAssigneeModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                projectTaskAssignee={selectedAssignee}
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
                        Tem certeza que deseja excluir o responsável <strong>{assigneeToDelete?.id}</strong>?
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

export default ProjectTaskAssigneeList;
