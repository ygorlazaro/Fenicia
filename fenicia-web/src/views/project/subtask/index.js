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
    CBadge,
    CFormCheck
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning } from '@coreui/icons';
import ProjectSubtaskClient from '../../../services/project-subtask-client';
import ProjectSubtaskModal from '../../../components/ProjectSubtaskModal';

const projectSubtaskClient = new ProjectSubtaskClient("http://localhost:5144");

const ProjectSubtaskList = () => {
    const [subtasks, setSubtasks] = useState([]);
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
    const [selectedSubtask, setSelectedSubtask] = useState(null);
    const [subtaskToDelete, setSubtaskToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadSubtasks();
    }, [pagination.page, pagination.perPage]);

    const loadSubtasks = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await projectSubtaskClient.getAll(pagination.page, pagination.perPage);
            console.log('Project Subtasks API response:', response);

            const subtasksList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Project Subtasks list:', subtasksList);
            setSubtasks(subtasksList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || subtasksList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load project subtasks:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar subtarefas do projeto.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedSubtask(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (subtask) => {
        setSelectedSubtask(subtask);
        setModalVisible(true);
    };

    const handleOpenDelete = (subtask) => {
        setSubtaskToDelete(subtask);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedSubtask) {
                await projectSubtaskClient.update(selectedSubtask.id, formData);
                setSuccessMessage('Subtarefa do projeto atualizada com sucesso!');
            } else {
                await projectSubtaskClient.create(formData);
                setSuccessMessage('Subtarefa do projeto criada com sucesso!');
            }
            setModalVisible(false);
            loadSubtasks();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save project subtask:', err);
            setError(err.response?.data?.title || 'Falha ao salvar subtarefa do projeto.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!subtaskToDelete) return;

        setDeleting(true);
        try {
            await projectSubtaskClient.delete(subtaskToDelete.id);
            setSuccessMessage('Subtarefa do projeto excluída com sucesso!');
            setDeleteModalVisible(false);
            setSubtaskToDelete(null);
            loadSubtasks();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete project subtask:', err);
            setError(err.response?.data?.title || 'Falha ao excluir subtarefa do projeto.');
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (page) => {
        setPagination(prev => ({ ...prev, page }));
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
                    <strong>Subtarefas do Projeto</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        Nova
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {loading && (
                        <div className="text-center py-4">
                            <CSpinner color="primary" />
                            <p className="mt-2">Carregando...</p>
                        </div>
                    )}

                    {!loading && subtasks.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhuma subtarefa de projeto cadastrada.</p>
                        </div>
                    )}

                    {!loading && subtasks.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Título</CTableHeaderCell>
                                        <CTableHeaderCell>Concluída</CTableHeaderCell>
                                        <CTableHeaderCell>Ordem</CTableHeaderCell>
                                        <CTableHeaderCell>Concluída Em</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {subtasks.map((subtask) => (
                                        <CTableRow key={subtask.id}>
                                            <CTableDataCell>
                                                <strong>{subtask.title}</strong>
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <CFormCheck
                                                    checked={subtask.isCompleted}
                                                    disabled
                                                />
                                            </CTableDataCell>
                                            <CTableDataCell>{subtask.order}</CTableDataCell>
                                            <CTableDataCell>{formatDate(subtask.completedAt)}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(subtask)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(subtask)}
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
                                    Mostrando {subtasks.length} de {pagination.total} registro(s)
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
            <ProjectSubtaskModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                projectSubtask={selectedSubtask}
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
                        Tem certeza que deseja excluir a subtarefa <strong>{subtaskToDelete?.title}</strong>?
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

export default ProjectSubtaskList;
