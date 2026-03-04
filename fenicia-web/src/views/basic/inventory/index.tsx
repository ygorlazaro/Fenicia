import React, { useEffect, useState } from 'react';
import {
    CButton, CCard, CCardBody, CCardHeader, CContainer, CTable, CTableBody, CTableDataCell,
    CTableHead, CTableHeaderCell, CTableRow, CModal, CModalBody, CModalFooter, CModalHeader,
    CModalTitle, CSpinner, CAlert, CForm, CFormInput, CFormLabel, CFormSelect, CRow, CCol
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPlus, cilWarning } from '@coreui/icons';
import { BasicInventoryClient, BasicProductClient, BasicProductCategoryClient } from '../../../services/basic-crud-clients';

const inventoryClient = new BasicInventoryClient();
const productClient = new BasicProductClient();
const categoryClient = new BasicProductCategoryClient();

const Inventory = () => {
    const [inventory, setInventory] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [filterType, setFilterType] = useState('all'); // all, product, category
    const [selectedProduct, setSelectedProduct] = useState('');
    const [selectedCategory, setSelectedCategory] = useState('');

    useEffect(() => {
        loadInventory();
        loadProducts();
        loadCategories();
    }, []);

    const loadInventory = async () => {
        try {
            setLoading(true);
            let response;
            if (filterType === 'product' && selectedProduct) {
                response = await inventoryClient.getByProduct(selectedProduct);
            } else if (filterType === 'category' && selectedCategory) {
                response = await inventoryClient.getByCategory(selectedCategory);
            } else {
                response = await inventoryClient.getAll();
            }
            
            const data = response?.data || Array.isArray(response) ? response : [];
            setInventory(Array.isArray(data) ? data : [data].filter(Boolean));
        } catch (err) {
            setError(err.response?.data?.title || 'Falha ao carregar estoque.');
        } finally {
            setLoading(false);
        }
    };

    const loadProducts = async () => {
        try {
            const response = await productClient.getAll(1, 100);
            const data = response?.data || Array.isArray(response) ? response : [];
            setProducts(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to load products:', err);
        }
    };

    const loadCategories = async () => {
        try {
            const data = await categoryClient.getAll();
            setCategories(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to load categories:', err);
        }
    };

    const handleFilter = () => {
        loadInventory();
    };

    const formatQuantity = (qty) => {
        return qty?.toFixed(2) || '0.00';
    };

    const getStockStatus = (item) => {
        const minStock = item.minStock || 0;
        const quantity = item.quantity || 0;
        
        if (quantity === 0) return { class: 'danger', text: 'Sem Estoque' };
        if (quantity < minStock) return { class: 'warning', text: 'Estoque Baixo' };
        return { class: 'success', text: 'Em Estoque' };
    };

    return (
        <CContainer className="py-4">
            {error && <CAlert color="danger" dismissible onClose={() => setError(null)}>{error}</CAlert>}
            
            <CCard>
                <CCardHeader>
                    <strong>Estoque</strong>
                </CCardHeader>
                <CCardBody>
                    {/* Filters */}
                    <CRow className="mb-4">
                        <CCol md={3}>
                            <CFormLabel>Filtrar por</CFormLabel>
                            <CFormSelect value={filterType} onChange={(e) => setFilterType(e.target.value)}>
                                <option value="all">Todos</option>
                                <option value="product">Por Produto</option>
                                <option value="category">Por Categoria</option>
                            </CFormSelect>
                        </CCol>
                        
                        {filterType === 'product' && (
                            <CCol md={4}>
                                <CFormLabel htmlFor="productFilter">Produto</CFormLabel>
                                <CFormSelect 
                                    id="productFilter" 
                                    value={selectedProduct} 
                                    onChange={(e) => setSelectedProduct(e.target.value)}
                                >
                                    <option value="">Selecione...</option>
                                    {products.map(p => (
                                        <option key={p.id} value={p.id}>{p.name}</option>
                                    ))}
                                </CFormSelect>
                            </CCol>
                        )}
                        
                        {filterType === 'category' && (
                            <CCol md={4}>
                                <CFormLabel htmlFor="categoryFilter">Categoria</CFormLabel>
                                <CFormSelect 
                                    id="categoryFilter" 
                                    value={selectedCategory} 
                                    onChange={(e) => setSelectedCategory(e.target.value)}
                                >
                                    <option value="">Selecione...</option>
                                    {categories.map(c => (
                                        <option key={c.id} value={c.id}>{c.name}</option>
                                    ))}
                                </CFormSelect>
                            </CCol>
                        )}
                        
                        <CCol md={3} className="d-flex align-items-end">
                            <CButton color="primary" onClick={handleFilter}>
                                Filtrar
                            </CButton>
                        </CCol>
                    </CRow>

                    {loading ? (
                        <div className="text-center py-4">
                            <CSpinner color="primary" />
                            <p className="mt-2">Carregando...</p>
                        </div>
                    ) : inventory.length === 0 ? (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhum registro de estoque encontrado.</p>
                        </div>
                    ) : (
                        <CTable hover responsive>
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>Produto</CTableHeaderCell>
                                    <CTableHeaderCell>Categoria</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">Quantidade</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">Mínimo</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">Máximo</CTableHeaderCell>
                                    <CTableHeaderCell>Status</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {inventory.map((item, idx) => {
                                    const status = getStockStatus(item);
                                    return (
                                        <CTableRow key={item.id || idx}>
                                            <CTableDataCell>{item.productName || item.product?.name || '-'}</CTableDataCell>
                                            <CTableDataCell>{item.categoryName || item.category?.name || '-'}</CTableDataCell>
                                            <CTableDataCell className="text-end">{formatQuantity(item.quantity)}</CTableDataCell>
                                            <CTableDataCell className="text-end">{formatQuantity(item.minStock)}</CTableDataCell>
                                            <CTableDataCell className="text-end">{formatQuantity(item.maxStock)}</CTableDataCell>
                                            <CTableDataCell>
                                                <span className={`badge bg-${status.class}`}>{status.text}</span>
                                            </CTableDataCell>
                                        </CTableRow>
                                    );
                                })}
                            </CTableBody>
                        </CTable>
                    )}
                </CCardBody>
            </CCard>
        </CContainer>
    );
};

export default Inventory;
