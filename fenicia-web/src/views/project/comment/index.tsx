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
import ProjectCommentClient from '../../../services/project-comment-client';
import ProjectCommentModal from '../../../components/ProjectCommentModal';
import Pagination from '../../../components/Pagination';

const projectCommentClient = new ProjectCommentClient("http://localhost:5144");

const ProjectCommentList = () => {
    const [comments, setComments] = useState([]);
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
    const [selectedComment, setSelectedComment] = useState(null);
    const [commentToDelete, setCommentToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadComments();
    }, [pagination.page, pagination.perPage]);

    const loadComments = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await projectCommentClient.getAll(pagination.page, pagination.perPage);
            console.log('Project Comments API response:', response);

            const commentsList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Project Comments list:', commentsList);
            setComments(commentsList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || commentsList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load project comments:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar comentários do projeto.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedComment(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (comment) => {
        setSelectedComment(comment);
        setModalVisible(true);
    };

    const handleOpenDelete = (comment) => {
        setCommentToDelete(comment);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedComment) {
                await projectCommentClient.update(selectedComment.id, formData);
                setSuccessMessage('Comentário do projeto atualizado com sucesso!');
            } else {
                await projectCommentClient.create(formData);
                setSuccessMessage('Comentário do projeto criado com sucesso!');
            }
            setModalVisible(false);
            loadComments();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save project comment:', err);
            setError(err.response?.data?.title || 'Falha ao salvar comentário do projeto.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!commentToDelete) return;

        setDeleting(true);
        try {
            await projectCommentClient.delete(commentToDelete.id);
            setSuccessMessage('Comentário do projeto excluído com sucesso!');
            setDeleteModalVisible(false);
            setCommentToDelete(null);
            loadComments();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete project comment:', err);
            setError(err.response?.data?.title || 'Falha ao excluir comentário do projeto.');
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

    const truncateContent = (content, maxLength = 100) => {
        if (!content) return '-';
        if (content.length <= maxLength) return content;
        return content.substring(0, maxLength) + '...';
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
                    <strong>Comentários do Projeto</strong>
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

                    {!loading && comments.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum comentário de projeto cadastrado.</p>
                        </div>
                    )}

                    {!loading && comments.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Conteúdo</CTableHeaderCell>
                                        <CTableHeaderCell>ID da Tarefa</CTableHeaderCell>
                                        <CTableHeaderCell>ID do Usuário</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {comments.map((comment) => (
                                        <CTableRow key={comment.id}>
                                            <CTableDataCell>
                                                <div style={{ maxWidth: '400px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                                                    {truncateContent(comment.content)}
                                                </div>
                                            </CTableDataCell>
                                            <CTableDataCell>{comment.taskId || '-'}</CTableDataCell>
                                            <CTableDataCell>{comment.userId || '-'}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(comment)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(comment)}
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
            <ProjectCommentModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                projectComment={selectedComment}
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
                        Tem certeza que deseja excluir o comentário <strong>{commentToDelete?.id}</strong>?
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

export default ProjectCommentList;
