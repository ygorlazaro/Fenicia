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
    CLink
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning, cilCloudDownload } from '@coreui/icons';
import ProjectAttachmentClient from '../../../services/project-attachment-client';
import ProjectAttachmentModal from '../../../components/ProjectAttachmentModal';
import Pagination from '../../../components/Pagination';

const projectAttachmentClient = new ProjectAttachmentClient("http://localhost:5144");

const ProjectAttachmentList = () => {
    const [attachments, setAttachments] = useState([]);
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
    const [selectedAttachment, setSelectedAttachment] = useState(null);
    const [attachmentToDelete, setAttachmentToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadAttachments();
    }, [pagination.page, pagination.perPage]);

    const loadAttachments = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await projectAttachmentClient.getAll(pagination.page, pagination.perPage);
            console.log('Project Attachments API response:', response);

            const attachmentsList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Project Attachments list:', attachmentsList);
            setAttachments(attachmentsList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || attachmentsList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load project attachments:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar anexos do projeto.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedAttachment(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (attachment) => {
        setSelectedAttachment(attachment);
        setModalVisible(true);
    };

    const handleOpenDelete = (attachment) => {
        setAttachmentToDelete(attachment);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedAttachment) {
                await projectAttachmentClient.update(selectedAttachment.id, formData);
                setSuccessMessage('Anexo do projeto atualizado com sucesso!');
            } else {
                await projectAttachmentClient.create(formData);
                setSuccessMessage('Anexo do projeto criado com sucesso!');
            }
            setModalVisible(false);
            loadAttachments();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save project attachment:', err);
            setError(err.response?.data?.title || 'Falha ao salvar anexo do projeto.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!attachmentToDelete) return;

        setDeleting(true);
        try {
            await projectAttachmentClient.delete(attachmentToDelete.id);
            setSuccessMessage('Anexo do projeto excluído com sucesso!');
            setDeleteModalVisible(false);
            setAttachmentToDelete(null);
            loadAttachments();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete project attachment:', err);
            setError(err.response?.data?.title || 'Falha ao excluir anexo do projeto.');
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

    const formatFileSize = (bytes) => {
        if (!bytes) return '-';
        const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
        if (bytes === 0) return '0 B';
        const i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)), 10);
        if (i === 0) return `${bytes} B`;
        return `${(bytes / (1024 ** i)).toFixed(2)} ${sizes[i]}`;
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
                    <strong>Anexos do Projeto</strong>
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

                    {!loading && attachments.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum anexo de projeto cadastrado.</p>
                        </div>
                    )}

                    {!loading && attachments.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Nome do Arquivo</CTableHeaderCell>
                                        <CTableHeaderCell>Tamanho</CTableHeaderCell>
                                        <CTableHeaderCell>ID da Tarefa</CTableHeaderCell>
                                        <CTableHeaderCell>Enviado Por</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {attachments.map((attachment) => (
                                        <CTableRow key={attachment.id}>
                                            <CTableDataCell>
                                                {attachment.fileUrl ? (
                                                    <CLink href={attachment.fileUrl} target="_blank" rel="noopener noreferrer">
                                                        <CIcon icon={cilCloudDownload} className="me-2" />
                                                        {attachment.fileName}
                                                    </CLink>
                                                ) : (
                                                    <strong>{attachment.fileName}</strong>
                                                )}
                                            </CTableDataCell>
                                            <CTableDataCell>{formatFileSize(attachment.fileSize)}</CTableDataCell>
                                            <CTableDataCell>{attachment.taskId || '-'}</CTableDataCell>
                                            <CTableDataCell>{attachment.uploadedBy || '-'}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(attachment)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(attachment)}
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
            <ProjectAttachmentModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                projectAttachment={selectedAttachment}
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
                        Tem certeza que deseja excluir o anexo <strong>{attachmentToDelete?.fileName}</strong>?
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

export default ProjectAttachmentList;
