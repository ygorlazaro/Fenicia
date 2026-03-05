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
import ProductCategoryModal from '../../../components/ProductCategoryModal';
import { BasicProductCategoryClient } from '../../../services/basic-crud-clients';
import Pagination from '../../../components/Pagination';

const categoryClient = new BasicProductCategoryClient();

const ProductCategories = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
    const [categories, setCategories] = useState([]);
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
    const [selectedCategory, setSelectedCategory] = useState(null);
    const [categoryToDelete, setCategoryToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const categoryId = searchParams.get('id');
        if (categoryId) {
            loadCategoryForEdit(categoryId);
        }
        loadCategories();
    }, [pagination.page, pagination.perPage]);

    const loadCategoryForEdit = async (categoryId) => {
        try {
            const category = await categoryClient.getById(categoryId);
            setSelectedCategory(category);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load category for edit:', err);
            setError(t('categories.loadError'));
        }
    };

    const loadCategories = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await categoryClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const categoriesList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? categoriesList.length;
            setCategories(categoriesList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            console.error('Failed to load categories:', err);
            setError(t('categories.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedCategory(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (category) => {
        try {
            const fullCategory = await categoryClient.getById(category.id);
            setSelectedCategory(fullCategory);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load category details:', err);
            setError(t('categories.loadError'));
        }
    };

    const handleOpenDelete = (category) => {
        setCategoryToDelete(category);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        setError(null);

        if (!formData.name) {
            setError(t('categories.requiredFields'));
            setSaving(false);
            return;
        }

        try {
            const payload = {
                id: selectedCategory?.id || crypto.randomUUID(),
                name: formData.name
            };

            if (selectedCategory) {
                await categoryClient.update(selectedCategory.id, payload);
                setSuccessMessage(t('categories.updateSuccess'));
            } else {
                await categoryClient.create(payload);
                setSuccessMessage(t('categories.createSuccess'));
            }
            setModalVisible(false);
            loadCategories();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save category:', err);
            setError(err.response?.data?.title || t('categories.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!categoryToDelete) return;

        setDeleting(true);
        try {
            await categoryClient.delete(categoryToDelete.id);
            setSuccessMessage(t('categories.deleteSuccess'));
            setDeleteModalVisible(false);
            setCategoryToDelete(null);
            loadCategories();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete category:', err);
            setError(t('categories.loadError'));
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
                    <strong>{t('categories.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('categories.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {loading && (
                        <div className="text-center py-4">
                            <CSpinner color="primary" />
                            <p className="mt-2">{t('common.loading')}</p>
                        </div>
                    )}

                    {!loading && categories.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">{t('common.noData')}</p>
                        </div>
                    )}

                    {!loading && categories.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t('categories.name')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {categories.map((category) => (
                                        <CTableRow key={category.id}>
                                            <CTableDataCell>{category.name}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton
                                                    color="info"
                                                    size="sm"
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(category)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton
                                                    color="danger"
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(category)}
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

            <ProductCategoryModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                category={selectedCategory}
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
                        {t('categories.deleteConfirm', { name: categoryToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('categories.deleteWarning')}
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

export default ProductCategories;
