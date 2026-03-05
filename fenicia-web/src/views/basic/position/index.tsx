import React, { useEffect, useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useSearchParams } from 'react-router-dom';
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
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
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

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const positionId = searchParams.get('id');
        if (positionId) {
            loadPositionForEdit(positionId);
        }
        loadPositions();
    }, [pagination.page, pagination.perPage]);

    const loadPositionForEdit = async (positionId) => {
        try {
            const position = await positionClient.getById(positionId);
            setSelectedPosition(position);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load position for edit:', err);
            setError(t('positions.loadError'));
        }
    };

    const loadPositions = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await positionClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const positionsList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? positionsList.length;
            setPositions(positionsList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            setError(t('positions.loadError'));
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
                setSuccessMessage(t('positions.updateSuccess'));
            } else {
                await positionClient.create(formData);
                setSuccessMessage(t('positions.createSuccess'));
            }
            setModalVisible(false);
            loadPositions();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('positions.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!positionToDelete) return;

        setDeleting(true);
        try {
            await positionClient.delete(positionToDelete.id);
            setSuccessMessage(t('positions.deleteSuccess'));
            setDeleteModalVisible(false);
            setPositionToDelete(null);
            loadPositions();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('positions.loadError'));
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (newPage) => {
        console.log('handlePageChange called with:', newPage);
        setPagination(prev => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage) => {
        console.log('handlePerPageChange called with:', newPerPage);
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
                    <strong>{t('positions.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('positions.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {loading && (
                        <div className="text-center py-4">
                            <CSpinner color="primary" />
                            <p className="mt-2">{t('common.loading')}</p>
                        </div>
                    )}

                    {!loading && positions.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">{t('common.noData')}</p>
                        </div>
                    )}

                    {!loading && positions.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t('positions.name')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {positions.map((position) => (
                                        <CTableRow key={position.id}>
                                            <CTableDataCell>{position.name}</CTableDataCell>
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

            <PositionModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                position={selectedPosition}
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
                        {t('positions.deleteConfirm', { name: positionToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('positions.deleteWarning')}
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

export default PositionList;
