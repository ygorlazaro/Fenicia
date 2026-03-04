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
    CBadge
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning } from '@coreui/icons';
import ProjectTaskClient from '../../../services/project-task-client';
import ProjectTaskModal from '../../../components/ProjectTaskModal';
import Pagination from '../../../components/Pagination';

const projectTaskClient = new ProjectTaskClient("http://localhost:5144");

const ProjectTaskList = () => {
    const [tasks, setTasks] = useState([]);
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
    const [selectedTask, setSelectedTask] = useState(null);
    const [taskToDelete, setTaskToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadTasks();
    }, [pagination.page, pagination.perPage]);

    const loadTasks = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await projectTaskClient.getAll(pagination.page, pagination.perPage);
            console.log('Project Tasks API response:', response);

            const tasksList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Project Tasks list:', tasksList);
            setTasks(tasksList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || tasksList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load project tasks:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar tarefas do projeto.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedTask(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (task) => {
        setSelectedTask(task);
        setModalVisible(true);
    };

    const handleOpenDelete = (task) => {
        setTaskToDelete(task);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedTask) {
                await projectTaskClient.update(selectedTask.id, formData);
                setSuccessMessage('Tarefa do projeto atualizada com sucesso!');
            } else {
                await projectTaskClient.create(formData);
                setSuccessMessage('Tarefa do projeto criada com sucesso!');
            }
            setModalVisible(false);
            loadTasks();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save project task:', err);
            setError(err.response?.data?.title || 'Falha ao salvar tarefa do projeto.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!taskToDelete) return;

        setDeleting(true);
        try {
            await projectTaskClient.delete(taskToDelete.id);
            setSuccessMessage('Tarefa do projeto excluída com sucesso!');
            setDeleteModalVisible(false);
            setTaskToDelete(null);
            loadTasks();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete project task:', err);
            setError(err.response?.data?.title || 'Falha ao excluir tarefa do projeto.');
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

    const getPriorityBadgeColor = (priority) => {
        const colorMap = {
            'Low': 'info',
            'Medium': 'warning',
            'High': 'danger',
            'Critical': 'dark'
        };
        return colorMap[priority] || 'secondary';
    };

    const getTypeBadgeColor = (type) => {
        const colorMap = {
            'Task': 'primary',
            'Bug': 'danger',
            'Feature': 'success',
            'Improvement': 'info',
            'Spike': 'warning'
        };
        return colorMap[type] || 'secondary';
    };

    const translatePriority = (priority) => {
        const translations = {
            'Low': 'Baixa',
            'Medium': 'Média',
            'High': 'Alta',
            'Critical': 'Crítica'
        };
        return translations[priority] || priority;
    };

    const translateType = (type) => {
        const translations = {
            'Task': 'Tarefa',
            'Bug': 'Bug',
            'Feature': 'Funcionalidade',
            'Improvement': 'Melhoria',
            'Spike': 'Spike'
        };
        return translations[type] || type;
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
                    <strong>Tarefas do Projeto</strong>
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

                    {!loading && tasks.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhuma tarefa de projeto cadastrada.</p>
                        </div>
                    )}

                    {!loading && tasks.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Título</CTableHeaderCell>
                                        <CTableHeaderCell>Prioridade</CTableHeaderCell>
                                        <CTableHeaderCell>Tipo</CTableHeaderCell>
                                        <CTableHeaderCell>Pontos</CTableHeaderCell>
                                        <CTableHeaderCell>Vencimento</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {tasks.map((task) => (
                                        <CTableRow key={task.id}>
                                            <CTableDataCell>
                                                <strong>{task.title}</strong>
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <CBadge color={getPriorityBadgeColor(task.priority)}>
                                                    {translatePriority(task.priority)}
                                                </CBadge>
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <CBadge color={getTypeBadgeColor(task.type)}>
                                                    {translateType(task.type)}
                                                </CBadge>
                                            </CTableDataCell>
                                            <CTableDataCell>{task.estimatePoints || '-'}</CTableDataCell>
                                            <CTableDataCell>{formatDate(task.dueDate)}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(task)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(task)}
                                                >
                                                    <CIcon icon={cilTrash} />
                                                </CButton>
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>

                            <Pagination pagination={pagination} onPageChange={handlePageChange} onPerPageChange={handlePerPageChange} />
                        </>
                    )}
                </CCardBody>
            </CCard>

            {/* Add/Edit Modal */}
            <ProjectTaskModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                projectTask={selectedTask}
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
                        Tem certeza que deseja excluir a tarefa <strong>{taskToDelete?.title}</strong>?
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

export default ProjectTaskList;
