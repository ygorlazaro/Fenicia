import React, { useEffect, useState } from 'react';
import {
    CButton, CCard, CCardBody, CCardHeader, CContainer, CTable, CTableBody, CTableDataCell,
    CTableHead, CTableHeaderCell, CTableRow, CModal, CModalBody, CModalFooter, CModalHeader,
    CModalTitle, CSpinner, CAlert, CForm, CFormInput, CFormLabel, CFormSelect, CRow, CCol,
    CFormTextarea
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilPlus, cilWarning } from '@coreui/icons';
import { BasicStockMovementClient, BasicProductClient } from '../../../services/basic-crud-clients';

const stockMovementClient = new BasicStockMovementClient();
const productClient = new BasicProductClient();

const StockMovement = () => {
    const [movements, setMovements] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [products, setProducts] = useState([]);
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedMovement, setSelectedMovement] = useState(null);
    const [saving, setSaving] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [formData, setFormData] = useState({
        productId: '',
        quantity: '',
        type: 'IN',
        notes: ''
    });

    useEffect(() => {
        loadMovements();
        loadProducts();
    }, []);

    const loadMovements = async () => {
        try {
            setLoading(true);
            const response = await stockMovementClient.getAll(startDate, endDate);
            const data = response?.data || Array.isArray(response) ? response : [];
            setMovements(Array.isArray(data) ? data : []);
        } catch (err) {
            setError(err.response?.data?.title || 'Falha ao carregar movimentações.');
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

    const handleOpenAdd = () => {
        setSelectedMovement(null);
        setFormData({ productId: '', quantity: '', type: 'IN', notes: '' });
        setModalVisible(true);
    };

    const handleOpenEdit = (movement) => {
        setSelectedMovement(movement);
        setFormData({
            productId: movement.productId || '',
            quantity: movement.quantity || '',
            type: movement.type || 'IN',
            notes: movement.notes || ''
        });
        setModalVisible(true);
    };

    const handleSave = async (e) => {
        e.preventDefault();
        setSaving(true);
        try {
            const payload = {
                ...formData,
                quantity: parseFloat(formData.quantity)
            };

            if (selectedMovement) {
                await stockMovementClient.update(selectedMovement.id, payload);
                setSuccessMessage('Movimentação atualizada com sucesso!');
            } else {
                await stockMovementClient.create(payload);
                setSuccessMessage('Movimentação criada com sucesso!');
            }
            setModalVisible(false);
            loadMovements();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(err.response?.data?.title || 'Falha ao salvar movimentação.');
        } finally {
            setSaving(false);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleFilter = () => {
        loadMovements();
    };

    const formatQuantity = (qty) => qty?.toFixed(2) || '0.00';

    const formatDate = (date) => {
        if (!date) return '-';
        return new Date(date).toLocaleDateString('pt-BR');
    };

    const getMovementTypeLabel = (type) => {
        const types = {
            'IN': { label: 'Entrada', class: 'success' },
            'OUT': { label: 'Saída', class: 'danger' },
            'TRANSFER': { label: 'Transferência', class: 'info' },
            'ADJUSTMENT': { label: 'Ajuste', class: 'warning' }
        };
        return types[type] || { label: type, class: 'secondary' };
    };

    return (
        <CContainer className="py-4">
            {error && <CAlert color="danger" dismissible onClose={() => setError(null)}>{error}</CAlert>}
            {successMessage && <CAlert color="success" dismissible onClose={() => setSuccessMessage(null)}>{successMessage}</CAlert>}
            
            <CCard>
                <CCardHeader className="d-flex justify-content-between align-items-center">
                    <strong>Movimentações de Estoque</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" /> Nova Movimentação
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Filters */}
                    <CRow className="mb-4">
                        <CCol md={3}>
                            <CFormLabel>Data Inicial</CFormLabel>
                            <CFormInput 
                                type="date" 
                                value={startDate} 
                                onChange={(e) => setStartDate(e.target.value)} 
                            />
                        </CCol>
                        <CCol md={3}>
                            <CFormLabel>Data Final</CFormLabel>
                            <CFormInput 
                                type="date" 
                                value={endDate} 
                                onChange={(e) => setEndDate(e.target.value)} 
                            />
                        </CCol>
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
                    ) : movements.length === 0 ? (
                        <div className="text-center py-4">
                            <p className="text-muted">Nenhuma movimentação encontrada.</p>
                        </div>
                    ) : (
                        <CTable hover responsive>
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>Data</CTableHeaderCell>
                                    <CTableHeaderCell>Produto</CTableHeaderCell>
                                    <CTableHeaderCell>Tipo</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">Quantidade</CTableHeaderCell>
                                    <CTableHeaderCell>Observações</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {movements.map((movement) => {
                                    const typeInfo = getMovementTypeLabel(movement.type);
                                    return (
                                        <CTableRow key={movement.id}>
                                            <CTableDataCell>{formatDate(movement.date)}</CTableDataCell>
                                            <CTableDataCell>{movement.productName || movement.product?.name || '-'}</CTableDataCell>
                                            <CTableDataCell>
                                                <span className={`badge bg-${typeInfo.class}`}>{typeInfo.label}</span>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">{formatQuantity(movement.quantity)}</CTableDataCell>
                                            <CTableDataCell>{movement.notes || '-'}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton 
                                                    color="info" 
                                                    size="sm"
                                                    onClick={() => handleOpenEdit(movement)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                            </CTableDataCell>
                                        </CTableRow>
                                    );
                                })}
                            </CTableBody>
                        </CTable>
                    )}
                </CCardBody>
            </CCard>

            {/* Add/Edit Modal */}
            <CModal visible={modalVisible} onClose={() => setModalVisible(false)} size="lg">
                <CModalHeader>
                    <CModalTitle>
                        {selectedMovement ? 'Editar Movimentação' : 'Nova Movimentação'}
                    </CModalTitle>
                </CModalHeader>
                <CForm onSubmit={handleSave}>
                    <CModalBody>
                        <CRow>
                            <CCol md={8}>
                                <div className="mb-3">
                                    <CFormLabel htmlFor="productId">Produto *</CFormLabel>
                                    <CFormSelect
                                        id="productId"
                                        name="productId"
                                        value={formData.productId}
                                        onChange={handleInputChange}
                                        required
                                    >
                                        <option value="">Selecione...</option>
                                        {products.map(p => (
                                            <option key={p.id} value={p.id}>{p.name}</option>
                                        ))}
                                    </CFormSelect>
                                </div>
                            </CCol>
                            <CCol md={4}>
                                <div className="mb-3">
                                    <CFormLabel htmlFor="type">Tipo *</CFormLabel>
                                    <CFormSelect
                                        id="type"
                                        name="type"
                                        value={formData.type}
                                        onChange={handleInputChange}
                                        required
                                    >
                                        <option value="IN">Entrada</option>
                                        <option value="OUT">Saída</option>
                                        <option value="TRANSFER">Transferência</option>
                                        <option value="ADJUSTMENT">Ajuste</option>
                                    </CFormSelect>
                                </div>
                            </CCol>
                        </CRow>
                        <div className="mb-3">
                            <CFormLabel htmlFor="quantity">Quantidade *</CFormLabel>
                            <CFormInput
                                type="number"
                                step="0.01"
                                min="0"
                                id="quantity"
                                name="quantity"
                                value={formData.quantity}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="mb-3">
                            <CFormLabel htmlFor="notes">Observações</CFormLabel>
                            <CFormTextarea
                                id="notes"
                                name="notes"
                                value={formData.notes}
                                onChange={handleInputChange}
                                rows={3}
                            />
                        </div>
                    </CModalBody>
                    <CModalFooter>
                        <CButton color="secondary" onClick={() => setModalVisible(false)} disabled={saving}>
                            Cancelar
                        </CButton>
                        <CButton color="primary" type="submit" disabled={saving}>
                            {saving ? 'Salvando...' : 'Salvar'}
                        </CButton>
                    </CModalFooter>
                </CForm>
            </CModal>
        </CContainer>
    );
};

export default StockMovement;
