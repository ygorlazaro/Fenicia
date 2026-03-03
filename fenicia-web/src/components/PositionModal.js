import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormTextarea,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const PositionModal = ({ 
    visible, 
    onClose, 
    onSave, 
    position, 
    loading 
}) => {
    const [formData, setFormData] = useState({
        name: '',
        description: '',
        code: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (position) {
            setFormData({
                name: position.name || '',
                description: position.description || '',
                code: position.code || ''
            });
        } else {
            setFormData({
                name: '',
                description: '',
                code: ''
            });
        }
        setError(null);
    }, [position, visible]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setError(null);

        // Validation
        if (!formData.name) {
            setError('Nome é obrigatório.');
            return;
        }

        onSave(formData);
    };

    return (
        <CModal 
            visible={visible} 
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {position ? 'Editar Cargo' : 'Novo Cargo'}
                </CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSubmit}>
                <CModalBody>
                    {error && (
                        <CAlert color="danger" dismissible>
                            {error}
                        </CAlert>
                    )}

                    <div className="mb-3">
                        <CFormLabel htmlFor="name">Nome *</CFormLabel>
                        <CFormInput
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleInputChange}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="code">Código</CFormLabel>
                        <CFormInput
                            type="text"
                            id="code"
                            name="code"
                            value={formData.code}
                            onChange={handleInputChange}
                        />
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="description">Descrição</CFormLabel>
                        <CFormTextarea
                            id="description"
                            name="description"
                            value={formData.description}
                            onChange={handleInputChange}
                            rows={3}
                        />
                    </div>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={onClose} disabled={loading}>
                        Cancelar
                    </CButton>
                    <CButton 
                        color="primary" 
                        type="submit"
                        disabled={loading}
                    >
                        {loading ? 'Salvando...' : 'Salvar'}
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default PositionModal;
