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
import { cilPencil, cilTrash, cilPlus, cilWarning, cilCalendar, cilDescription } from '@coreui/icons';
import ProjectClient from '../../../services/project-client';
import ProjectModal from '../../../components/ProjectModal';
import Pagination from '../../../components/Pagination';

const projectClient = new ProjectClient("http://localhost:5144");

const ProjectList = () => {
    const [projects, setProjects] = useState([]);
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
    const [selectedProject, setSelectedProject] = useState(null);
    const [projectToDelete, setProjectToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadProjects();
    }, [pagination.page, pagination.perPage]);

    const loadProjects = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await projectClient.getAll(pagination.page, pagination.perPage);
            console.log('Projects API response:', response);

            const projectsList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Projects list:', projectsList);
            setProjects(projectsList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || projectsList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load projects:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar projetos.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedProject(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (project) => {
        setSelectedProject(project);
        setModalVisible(true);
    };

    const handleOpenDelete = (project) => {
        setProjectToDelete(project);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedProject) {
                await projectClient.update(selectedProject.id, formData);
                setSuccessMessage('Projeto atualizado com sucesso!');
            } else {
                await projectClient.create(formData);
                setSuccessMessage('Projeto criado com sucesso!');
            }
            setModalVisible(false);
            loadProjects();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save project:', err);
            setError(err.response?.data?.title || 'Falha ao salvar projeto.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!projectToDelete) return;

        setDeleting(true);
        try {
            await projectClient.delete(projectToDelete.id);
            setSuccessMessage('Projeto excluído com sucesso!');
            setDeleteModalVisible(false);
            setProjectToDelete(null);
            loadProjects();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete project:', err);
            setError(err.response?.data?.title || 'Falha ao excluir projeto.');
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

    const getStatusColor = (status) => {
        const colors = {
            Draft: 'secondary',
            Active: 'success',
            Archived: 'dark',
            Completed: 'info'
        };
        return colors[status] || 'secondary';
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
                    <strong>Projetos</strong>
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

                    {!loading && projects.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum projeto cadastrado.</p>
                        </div>
                    )}

                    {!loading && projects.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Título</CTableHeaderCell>
                                        <CTableHeaderCell>Descrição</CTableHeaderCell>
                                        <CTableHeaderCell>Status</CTableHeaderCell>
                                        <CTableHeaderCell>Início</CTableHeaderCell>
                                        <CTableHeaderCell>Fim</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {projects.map((project) => (
                                        <CTableRow key={project.id}>
                                            <CTableDataCell>
                                                <strong>{project.title}</strong>
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <div style={{ maxWidth: '300px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                                                    {project.description || '-'}
                                                </div>
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <CBadge color={getStatusColor(project.status)}>
                                                    {project.status === 'Active' ? 'Ativo' : 
                                                     project.status === 'Completed' ? 'Concluído' :
                                                     project.status === 'Archived' ? 'Arquivado' : 'Rascunho'}
                                                </CBadge>
                                            </CTableDataCell>
                                            <CTableDataCell>{formatDate(project.startDate)}</CTableDataCell>
                                            <CTableDataCell>{formatDate(project.endDate)}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(project)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(project)}
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
            <ProjectModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                project={selectedProject}
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
                        Tem certeza que deseja excluir o projeto <strong>{projectToDelete?.title}</strong>?
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

export default ProjectList;
