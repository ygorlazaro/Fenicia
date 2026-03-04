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
import BasicPositionClient from '../../../services/basic-position-client';
import PositionModal from '../../../components/PositionModal';
import Pagination from '../../../components/Pagination';

const positionClient = new BasicPositionClient("http://localhost:5083");

const PositionList = () => {
    const [positions, setPositions] = useState([]);
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
    const [selectedPosition, setSelectedPosition] = useState(null);
    const [positionToDelete, setPositionToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    useEffect(() => {
        loadPositions();
    }, [pagination.page, pagination.perPage]);

    const loadPositions = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await positionClient.getAll(pagination.page, pagination.perPage);
            console.log('Positions API response:', response);
            
            // API might return array directly or Pagination object
            const positionsList = Array.isArray(response) ? response : (response?.data || []);
            console.log('Positions list:', positionsList);
            setPositions(positionsList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || positionsList.length,
                pages: response?.pages || 1
            }));
        } catch (err) {
            console.error('Failed to load positions:', err);
            console.error('Error response:', err.response);
            setError(err.response?.data?.title || 'Falha ao carregar cargos.');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedPosition(null);
        setModalVisible(true);
    };

    const handleOpenEdit = (position) => {
        setSelectedPosition(position);
        setModalVisible(true);
    };

    const handleOpenDelete = (position) => {
        setPositionToDelete(position);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedPosition) {
                await positionClient.update(selectedPosition.id, formData);
                setSuccessMessage('Cargo atualizado com sucesso!');
            } else {
                await positionClient.create(formData);
                setSuccessMessage('Cargo criado com sucesso!');
            }
            setModalVisible(false);
            loadPositions();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save position:', err);
            setError(err.response?.data?.title || 'Falha ao salvar cargo.');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!positionToDelete) return;

        setDeleting(true);
        try {
            await positionClient.delete(positionToDelete.id);
            setSuccessMessage('Cargo excluído com sucesso!');
            setDeleteModalVisible(false);
            setPositionToDelete(null);
            loadPositions();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete position:', err);
            setError(err.response?.data?.title || 'Falha ao excluir cargo.');
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
                    <strong>Cargos</strong>
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

                    {!loading && positions.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum cargo cadastrado.</p>
                        </div>
                    )}

                    {!loading && positions.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Nome</CTableHeaderCell>
                                        <CTableHeaderCell>Código</CTableHeaderCell>
                                        <CTableHeaderCell>Descrição</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {positions.map((position) => (
                                        <CTableRow key={position.id}>
                                            <CTableDataCell>{position.name}</CTableDataCell>
                                            <CTableDataCell>{position.code || '-'}</CTableDataCell>
                                            <CTableDataCell>{position.description || '-'}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton 
                                                    color="info" 
                                                    size="sm" 
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(position)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton 
                                                    color="danger" 
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(position)}
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
            <PositionModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                position={selectedPosition}
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
                        Tem certeza que deseja excluir o cargo <strong>{positionToDelete?.name}</strong>?
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

export default PositionList;
