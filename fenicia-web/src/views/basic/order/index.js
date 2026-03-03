import React, { useEffect, useState } from 'react';
import {
    CButton, CCard, CCardBody, CCardHeader, CContainer, CTable, CTableBody, CTableDataCell,
    CTableHead, CTableHeaderCell, CTableRow, CModal, CModalBody, CModalFooter, CModalHeader,
    CModalTitle, CSpinner, CAlert, CForm, CFormInput, CFormLabel, CFormSelect, CRow, CCol
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPlus } from '@coreui/icons';
import { BasicOrderClient, BasicProductClient, BasicCustomerClient } from '../../../services/basic-crud-clients';

const orderClient = new BasicOrderClient();
const productClient = new BasicProductClient();
const customerClient = new BasicCustomerClient();

const Orders = () => {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [products, setProducts] = useState([]);
    const [customers, setCustomers] = useState([]);
    const [modalVisible, setModalVisible] = useState(false);
    const [saving, setSaving] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);
    const [orderItems, setOrderItems] = useState([]);
    const [selectedProduct, setSelectedProduct] = useState('');
    const [quantity, setQuantity] = useState(1);

    useEffect(() => {
        loadProducts();
        loadCustomers();
    }, []);

    const loadProducts = async () => {
        try {
            const response = await productClient.getAll(1, 100);
            const data = response?.data || Array.isArray(response) ? response : [];
            setProducts(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to load products:', err);
        }
    };

    const loadCustomers = async () => {
        try {
            const response = await customerClient.getAll(1, 100);
            const data = response?.data || Array.isArray(response) ? response : [];
            setCustomers(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to load customers:', err);
        }
    };

    const handleOpenAdd = () => {
        setOrderItems([]);
        setSelectedProduct('');
        setQuantity(1);
        setModalVisible(true);
    };

    const handleAddItem = () => {
        if (!selectedProduct || quantity <= 0) return;

        const product = products.find(p => p.id === selectedProduct);
        if (!product) return;

        const existingItem = orderItems.find(item => item.productId === selectedProduct);
        if (existingItem) {
            setOrderItems(orderItems.map(item =>
                item.productId === selectedProduct
                    ? { ...item, quantity: item.quantity + parseFloat(quantity) }
                    : item
            ));
        } else {
            setOrderItems([...orderItems, {
                productId: selectedProduct,
                productName: product.name,
                price: product.price || 0,
                quantity: parseFloat(quantity)
            }]);
        }

        setSelectedProduct('');
        setQuantity(1);
    };

    const handleRemoveItem = (productId) => {
        setOrderItems(orderItems.filter(item => item.productId !== productId));
    };

    const handleSave = async (e) => {
        e.preventDefault();
        
        if (orderItems.length === 0) {
            setError('Adicione pelo menos um item ao pedido.');
            return;
        }

        setSaving(true);
        try {
            const payload = {
                items: orderItems.map(item => ({
                    productId: item.productId,
                    quantity: item.quantity,
                    price: item.price
                }))
            };

            await orderClient.create(payload);
            setSuccessMessage('Pedido criado com sucesso!');
            setModalVisible(false);
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(err.response?.data?.title || 'Falha ao criar pedido.');
        } finally {
            setSaving(false);
        }
    };

    const calculateTotal = () => {
        return orderItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    };

    const formatCurrency = (value) => {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    };

    return (
        <CContainer className="py-4">
            {error && <CAlert color="danger" dismissible onClose={() => setError(null)}>{error}</CAlert>}
            {successMessage && <CAlert color="success" dismissible onClose={() => setSuccessMessage(null)}>{successMessage}</CAlert>}
            
            <CCard>
                <CCardHeader className="d-flex justify-content-between align-items-center">
                    <strong>Pedidos</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" /> Novo Pedido
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    <div className="text-center py-4">
                        <p className="text-muted">
                            Funcionalidade de listagem de pedidos em desenvolvimento.
                            <br />
                            Utilize o botão "Novo Pedido" para criar um novo pedido.
                        </p>
                    </div>
                </CCardBody>
            </CCard>

            {/* Add Order Modal */}
            <CModal visible={modalVisible} onClose={() => setModalVisible(false)} size="xl">
                <CModalHeader>
                    <CModalTitle>Novo Pedido</CModalTitle>
                </CModalHeader>
                <CForm onSubmit={handleSave}>
                    <CModalBody>
                        <h6 className="mb-3">Adicionar Itens</h6>
                        
                        <CRow className="mb-4">
                            <CCol md={6}>
                                <CFormLabel htmlFor="product">Produto *</CFormLabel>
                                <CFormSelect
                                    id="product"
                                    value={selectedProduct}
                                    onChange={(e) => setSelectedProduct(e.target.value)}
                                >
                                    <option value="">Selecione...</option>
                                    {products.map(p => (
                                        <option key={p.id} value={p.id}>
                                            {p.name} - {formatCurrency(p.price || 0)}
                                        </option>
                                    ))}
                                </CFormSelect>
                            </CCol>
                            <CCol md={3}>
                                <CFormLabel htmlFor="quantity">Quantidade *</CFormLabel>
                                <CFormInput
                                    type="number"
                                    min="1"
                                    step="1"
                                    id="quantity"
                                    value={quantity}
                                    onChange={(e) => setQuantity(e.target.value)}
                                />
                            </CCol>
                            <CCol md={3} className="d-flex align-items-end">
                                <CButton color="primary" onClick={handleAddItem} disabled={!selectedProduct}>
                                    <CIcon icon={cilPlus} className="me-2" /> Adicionar
                                </CButton>
                            </CCol>
                        </CRow>

                        <h6 className="mb-3">Itens do Pedido</h6>
                        
                        {orderItems.length === 0 ? (
                            <div className="text-center py-4">
                                <p className="text-muted">Nenhum item adicionado.</p>
                            </div>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>Produto</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Preço Unit.</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Qtd.</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Total</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {orderItems.map((item) => (
                                        <CTableRow key={item.productId}>
                                            <CTableDataCell>{item.productName}</CTableDataCell>
                                            <CTableDataCell className="text-end">{formatCurrency(item.price)}</CTableDataCell>
                                            <CTableDataCell className="text-end">{item.quantity}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                {formatCurrency(item.price * item.quantity)}
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton 
                                                    color="danger" 
                                                    size="sm"
                                                    onClick={() => handleRemoveItem(item.productId)}
                                                >
                                                    Remover
                                                </CButton>
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                                <CTableFoot>
                                    <CTableRow>
                                        <CTableHeaderCell colSpan={3} className="text-end">Total Geral:</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">
                                            <strong>{formatCurrency(calculateTotal())}</strong>
                                        </CTableHeaderCell>
                                        <CTableHeaderCell></CTableHeaderCell>
                                    </CTableRow>
                                </CTableFoot>
                            </CTable>
                        )}
                    </CModalBody>
                    <CModalFooter>
                        <CButton color="secondary" onClick={() => setModalVisible(false)} disabled={saving}>
                            Cancelar
                        </CButton>
                        <CButton color="primary" type="submit" disabled={saving || orderItems.length === 0}>
                            {saving ? 'Salvando...' : 'Criar Pedido'}
                        </CButton>
                    </CModalFooter>
                </CForm>
            </CModal>
        </CContainer>
    );
};

export default Orders;
