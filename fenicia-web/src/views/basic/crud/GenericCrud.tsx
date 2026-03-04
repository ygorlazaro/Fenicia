import React, { useEffect, useState } from 'react';
import {
    CButton, CCard, CCardBody, CCardHeader, CContainer, CTable, CTableBody, CTableDataCell,
    CTableHead, CTableHeaderCell, CTableRow, CModal, CModalBody, CModalFooter, CModalHeader,
    CModalTitle, CSpinner, CAlert, CForm, CFormInput, CFormLabel,
    CFormSelect, CFormTextarea, CRow, CCol, CInputGroup, CInputGroupText
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPencil, cilTrash, cilPlus, cilWarning, cilSearch } from '@coreui/icons';
import { BasicCustomerClient, BasicSupplierClient, BasicProductCategoryClient, BasicProductClient } from '../../../services/basic-crud-clients';
import { fetchAddressByCep } from '../../../services/cep-client';
import Pagination from '../../../components/Pagination';

const customerClient = new BasicCustomerClient();
const supplierClient = new BasicSupplierClient();
const categoryClient = new BasicProductCategoryClient();

const GenericCrud = ({ entityType, client, columns, formFields, title }) => {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [pagination, setPagination] = useState({ page: 1, perPage: 10, total: 0, pages: 0 });
    const [modalVisible, setModalVisible] = useState(false);
    const [deleteModalVisible, setDeleteModalVisible] = useState(false);
    const [selectedItem, setSelectedItem] = useState(null);
    const [itemToDelete, setItemToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);
    const [formData, setFormData] = useState({});
    const [categories, setCategories] = useState([]);
    const [suppliers, setSuppliers] = useState([]);

    useEffect(() => {
        loadItems();
        if (entityType === 'product') {
            loadCategories();
            loadSuppliers();
        }
    }, [pagination.page]);

    const loadItems = async () => {
        try {
            setLoading(true);
            const response = await client.getAll(pagination.page, pagination.perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const itemsList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            setItems(itemsList);
            setPagination(prev => ({
                ...prev,
                total: response?.total || itemsList.length,
                pages: Math.ceil((response?.total || itemsList.length) / prev.perPage) || 1
            }));
        } catch (err) {
            setError(err.response?.data?.title || `Falha ao carregar ${title}.`);
        } finally {
            setLoading(false);
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

    const loadSuppliers = async () => {
        try {
            const data = await supplierClient.getAll(1, 100);
            setSuppliers(Array.isArray(data) ? data : (data?.data || []));
        } catch (err) {
            console.error('Failed to load suppliers:', err);
        }
    };

    const handleOpenAdd = () => {
        setSelectedItem(null);
        setFormData({});
        setModalVisible(true);
    };

    const handleOpenEdit = (item) => {
        setSelectedItem(item);
        setFormData(item);
        setModalVisible(true);
    };

    const handleOpenDelete = (item) => {
        setItemToDelete(item);
        setDeleteModalVisible(true);
    };

    const handleSave = async (e) => {
        e.preventDefault();
        setSaving(true);
        try {
            if (selectedItem) {
                await client.update(selectedItem.id, formData);
                setSuccessMessage(`${title.slice(0, -1)} atualizado com sucesso!`);
            } else {
                await client.create(formData);
                setSuccessMessage(`${title.slice(0, -1)} criado com sucesso!`);
            }
            setModalVisible(false);
            loadItems();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(err.response?.data?.title || `Falha ao salvar ${title.slice(0, -1)}.`);
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!itemToDelete) return;
        setDeleting(true);
        try {
            await client.delete(itemToDelete.id);
            setSuccessMessage(`${title.slice(0, -1)} excluído com sucesso!`);
            setDeleteModalVisible(false);
            loadItems();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(err.response?.data?.title || `Falha ao excluir ${title.slice(0, -1)}.`);
        } finally {
            setDeleting(false);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleCepChange = async (e) => {
        const { name, value } = e.target;
        const cleanCep = value.replace(/\D/g, '');
        setFormData(prev => ({ ...prev, [name]: value }));

        if (cleanCep.length === 8) {
            const address = await fetchAddressByCep(cleanCep);
            if (address) {
                setFormData(prev => ({
                    ...prev,
                    [name]: address.cep,
                    state: address.state,
                    city: address.city,
                    neighborhood: address.neighborhood,
                    street: address.street,
                    complement: address.complement || prev.complement || ''
                }));
            }
        }
    };

    return (
        <CContainer className="py-4">
            {error && <CAlert color="danger" dismissible onClose={() => setError(null)}>{error}</CAlert>}
            {successMessage && <CAlert color="success" dismissible onClose={() => setSuccessMessage(null)}>{successMessage}</CAlert>}
            
            <CCard>
                <CCardHeader className="d-flex justify-content-between align-items-center">
                    <strong>{title}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" /> Novo
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {loading ? (
                        <div className="text-center py-4"><CSpinner color="primary" /><p className="mt-2">Carregando...</p></div>
                    ) : items.length === 0 ? (
                        <div className="text-center py-4"><p className="text-muted">Nenhum registro encontrado.</p></div>
                    ) : (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        {columns.map((col, idx) => <CTableHeaderCell key={idx}>{col.header}</CTableHeaderCell>)}
                                        <CTableHeaderCell className="text-end">Ações</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {items.map((item) => (
                                        <CTableRow key={item.id}>
                                            {columns.map((col, idx) => (
                                                <CTableDataCell key={idx}>{col.render ? col.render(item[col.field], item) : item[col.field] || '-'}</CTableDataCell>
                                            ))}
                                            <CTableDataCell className="text-end">
                                                <CButton color="info" size="sm" className="me-2" onClick={() => handleOpenEdit(item)}>
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton color="danger" size="sm" onClick={() => handleOpenDelete(item)}>
                                                    <CIcon icon={cilTrash} />
                                                </CButton>
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>
                            <Pagination
                                pagination={pagination}
                                onPageChange={(newPage) => setPagination(p => ({ ...p, page: newPage }))}
                                onPerPageChange={(newPerPage) => setPagination(p => ({ ...p, perPage: newPerPage, page: 1 }))}
                            />
                        </>
                    )}
                </CCardBody>
            </CCard>

            <CModal visible={modalVisible} onClose={() => setModalVisible(false)} size="lg">
                <CModalHeader><CModalTitle>{selectedItem ? `Editar ${title.slice(0, -1)}` : `Novo ${title.slice(0, -1)}`}</CModalTitle></CModalHeader>
                <CForm onSubmit={handleSave}>
                    <CModalBody>
                        {formFields.map((field, idx) => (
                            field.type === 'select' ? (
                                <div className="mb-3" key={idx}>
                                    <CFormLabel htmlFor={field.name}>{field.label}{field.required ? ' *' : ''}</CFormLabel>
                                    <CFormSelect id={field.name} name={field.name} value={formData[field.name] || ''} onChange={handleInputChange} required={field.required}>
                                        <option value="">Selecione...</option>
                                        {field.options?.map(opt => <option key={opt.value} value={opt.value}>{opt.label}</option>)}
                                    </CFormSelect>
                                </div>
                            ) : field.type === 'cep' ? (
                                <div className="mb-3" key={idx}>
                                    <CFormLabel htmlFor={field.name}>{field.label}{field.required ? ' *' : ''}</CFormLabel>
                                    <CInputGroup>
                                        <CFormInput 
                                            type="text" 
                                            id={field.name} 
                                            name={field.name} 
                                            value={formData[field.name] || ''} 
                                            onChange={handleCepChange}
                                            placeholder="00000-000"
                                            required={field.required}
                                            maxLength={9}
                                        />
                                        <CInputGroupText style={{ cursor: 'pointer' }} onClick={async () => {
                                            const cleanCep = (formData[field.name] || '').replace(/\D/g, '');
                                            if (cleanCep.length === 8) {
                                                const address = await fetchAddressByCep(cleanCep);
                                                if (address) {
                                                    setFormData(prev => ({
                                                        ...prev,
                                                        state: address.state,
                                                        city: address.city,
                                                        neighborhood: address.neighborhood,
                                                        street: address.street,
                                                        complement: address.complement || prev.complement || ''
                                                    }));
                                                }
                                            }
                                        }}>
                                            <CIcon icon={cilSearch} />
                                        </CInputGroupText>
                                    </CInputGroup>
                                </div>
                            ) : field.type === 'textarea' ? (
                                <div className="mb-3" key={idx}>
                                    <CFormLabel htmlFor={field.name}>{field.label}{field.required ? ' *' : ''}</CFormLabel>
                                    <CFormTextarea id={field.name} name={field.name} value={formData[field.name] || ''} onChange={handleInputChange} rows={3} required={field.required} />
                                </div>
                            ) : (
                                <div className="mb-3" key={idx}>
                                    <CFormLabel htmlFor={field.name}>{field.label}{field.required ? ' *' : ''}</CFormLabel>
                                    <CFormInput type={field.type || 'text'} id={field.name} name={field.name} value={formData[field.name] || ''} onChange={handleInputChange} required={field.required} />
                                </div>
                            )
                        ))}
                    </CModalBody>
                    <CModalFooter>
                        <CButton color="secondary" onClick={() => setModalVisible(false)} disabled={saving}>Cancelar</CButton>
                        <CButton color="primary" type="submit" disabled={saving}>{saving ? 'Salvando...' : 'Salvar'}</CButton>
                    </CModalFooter>
                </CForm>
            </CModal>

            <CModal visible={deleteModalVisible} onClose={() => setDeleteModalVisible(false)}>
                <CModalHeader><CModalTitle><CIcon icon={cilWarning} className="me-2 text-warning" />Confirmar Exclusão</CModalTitle></CModalHeader>
                <CModalBody>
                    <p>Tem certeza que deseja excluir <strong>{itemToDelete?.name}</strong>?</p>
                    <p className="text-danger">Esta ação não pode ser desfeita.</p>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={() => setDeleteModalVisible(false)} disabled={deleting}>Cancelar</CButton>
                    <CButton color="danger" onClick={handleDelete} disabled={deleting}>{deleting ? 'Excluindo...' : 'Excluir'}</CButton>
                </CModalFooter>
            </CModal>
        </CContainer>
    );
};

// Customer configuration
const CustomerCrud = () => (
    <GenericCrud
        entityType="customer"
        client={customerClient}
        title="Clientes"
        columns={[
            { field: 'name', header: 'Nome' },
            { field: 'email', header: 'E-mail' },
            { field: 'phone', header: 'Telefone', render: (v) => v || '-' },
            { field: 'document', header: 'Documento', render: (v) => v || '-' }
        ]}
        formFields={[
            { name: 'name', label: 'Nome', required: true },
            { name: 'email', label: 'E-mail', type: 'email', required: true },
            { name: 'phone', label: 'Telefone' },
            { name: 'document', label: 'Documento' },
            { name: 'zipCode', label: 'CEP', type: 'cep' },
            { name: 'state', label: 'Estado' },
            { name: 'city', label: 'Cidade' },
            { name: 'neighborhood', label: 'Bairro' },
            { name: 'street', label: 'Rua' },
            { name: 'number', label: 'Número' },
            { name: 'complement', label: 'Complemento' },
            { name: 'address', label: 'Endereço', type: 'textarea' }
        ]}
    />
);

// Supplier configuration
const SupplierCrud = () => (
    <GenericCrud
        entityType="supplier"
        client={supplierClient}
        title="Fornecedores"
        columns={[
            { field: 'name', header: 'Nome' },
            { field: 'email', header: 'E-mail' },
            { field: 'phone', header: 'Telefone', render: (v) => v || '-' },
            { field: 'document', header: 'Documento', render: (v) => v || '-' }
        ]}
        formFields={[
            { name: 'name', label: 'Nome', required: true },
            { name: 'email', label: 'E-mail', type: 'email', required: true },
            { name: 'phone', label: 'Telefone' },
            { name: 'document', label: 'Documento' },
            { name: 'zipCode', label: 'CEP', type: 'cep' },
            { name: 'state', label: 'Estado' },
            { name: 'city', label: 'Cidade' },
            { name: 'neighborhood', label: 'Bairro' },
            { name: 'street', label: 'Rua' },
            { name: 'number', label: 'Número' },
            { name: 'complement', label: 'Complemento' },
            { name: 'address', label: 'Endereço', type: 'textarea' }
        ]}
    />
);

// Product Category configuration
const ProductCategoryCrud = () => (
    <GenericCrud
        entityType="productCategory"
        client={categoryClient}
        title="Categorias de Produto"
        columns={[
            { field: 'name', header: 'Nome' },
            { field: 'code', header: 'Código', render: (v) => v || '-' },
            { field: 'description', header: 'Descrição', render: (v) => v || '-' }
        ]}
        formFields={[
            { name: 'name', label: 'Nome', required: true },
            { name: 'code', label: 'Código' },
            { name: 'description', label: 'Descrição', type: 'textarea' }
        ]}
    />
);

// Product configuration
const ProductCrud = () => {
    const productClient = new BasicProductClient();
    return (
        <GenericCrud
            entityType="product"
            client={productClient}
            title="Produtos"
            columns={[
                { field: 'name', header: 'Nome' },
                { field: 'sku', header: 'SKU', render: (v) => v || '-' },
                { field: 'price', header: 'Preço', render: (v) => v ? `R$ ${v.toFixed(2)}` : '-' },
                { field: 'categoryId', header: 'Categoria', render: (_, item) => item.categoryName || '-' }
            ]}
            formFields={[
                { name: 'name', label: 'Nome', required: true },
                { name: 'sku', label: 'SKU' },
                { name: 'description', label: 'Descrição', type: 'textarea' },
                { name: 'price', label: 'Preço', type: 'number' },
                { name: 'cost', label: 'Custo', type: 'number' },
                { name: 'categoryId', label: 'Categoria', type: 'select', options: [], required: true },
                { name: 'supplierId', label: 'Fornecedor', type: 'select', options: [] }
            ]}
        />
    );
};

export { CustomerCrud, SupplierCrud, ProductCategoryCrud, ProductCrud };
