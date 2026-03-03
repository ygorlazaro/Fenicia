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
    CFormSwitch
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning } from '@coreui/icons';
import ProjectStatusClient from '../../../services/project-status-client';
import ProjectStatusModal from '../../../components/ProjectStatusModal';

const projectStatusClient = new ProjectStatusClient("http://localhost:5144");

const ProjectStatusList = () => {
    const [statuses, setStatuses] = useState([]);
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
    const [selectedStatus, setSelectedStatus] = useState(null);
    const [statusToDelete, setStatusToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadStatuses();
    }, [pagination.page, pagination.perPage]);

    const loadStatuses = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await projectStatusClient.getAll(pagination.page, pagination.perPage);
            console.log('Project Statuses API response:', response);

            const statusesList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Project Statuses list:', statusesList);
            setStatuses(statusesList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || statusesList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load project statuses:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar status dos projetos.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedStatus(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (status) => {
        setSelectedStatus(status);
        setModalVisible(true);
    };

    const handleOpenDelete = (status) => {
        setStatusToDelete(status);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedStatus) {
                await projectStatusClient.update(selectedStatus.id, formData);
                setSuccessMessage('Status do projeto atualizado com sucesso!');
            } else {
                await projectStatusClient.create(formData);
                setSuccessMessage('Status do projeto criado com sucesso!');
            }
            setModalVisible(false);
            loadStatuses();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save project status:', err);
            setError(err.response?.data?.title || 'Falha ao salvar status do projeto.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!statusToDelete) return;

        setDeleting(true);
        try {
            await projectStatusClient.delete(statusToDelete.id);
            setSuccessMessage('Status do projeto excluído com sucesso!');
            setDeleteModalVisible(false);
            setStatusToDelete(null);
            loadStatuses();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete project status:', err);
            setError(err.response?.data?.title || 'Falha ao excluir status do projeto.');
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (page) => {
        setPagination(prev => ({ ...prev, page }));
    };

    const getStatusBadgeColor = (color) => {
        const colorMap = {
            'primary': 'primary',
            'secondary': 'secondary',
            'success': 'success',
            'danger': 'danger',
            'warning': 'warning',
            'info': 'info',
            'dark': 'dark',
            'light': 'light'
        };
        return colorMap[color?.toLowerCase()] || 'secondary';
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
                    <strong>Status dos Projetos</strong>
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

                    {!loading && statuses.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum status de projeto cadastrado.</p>
                        </div>
                    )}

                    {!loading && statuses.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Nome</CTableHeaderCell>
                                        <CTableHeaderCell>Cor</CTableHeaderCell>
                                        <CTableHeaderCell>Ordem</CTableHeaderCell>
                                        <CTableHeaderCell>Final</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {statuses.map((status) => (
                                        <CTableRow key={status.id}>
                                            <CTableDataCell>
                                                <strong>{status.name}</strong>
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <CBadge color={getStatusBadgeColor(status.color)}>
                                                    {status.color}
                                                </CBadge>
                                            </CTableDataCell>
                                            <CTableDataCell>{status.order}</CTableDataCell>
                                            <CTableDataCell>
                                                <CFormSwitch
                                                    checked={status.isFinal}
                                                    disabled
                                                />
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(status)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(status)}
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
                                    Mostrando {statuses.length} de {pagination.total} registro(s)
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
            <ProjectStatusModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                projectStatus={selectedStatus}
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
                        Tem certeza que deseja excluir o status <strong>{statusToDelete?.name}</strong>?
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

export default ProjectStatusList;
