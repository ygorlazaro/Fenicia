import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormSelect,
    CFormSwitch,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProjectStatusModal = ({
    visible,
    onClose,
    onSave,
    projectStatus,
    loading
}) => {
    const [formData, setFormData] = useState({
        name: '',
        color: 'primary',
        order: 0,
        isFinal: false,
        projectId: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (projectStatus) {
            setFormData({
                name: projectStatus.name || '',
                color: projectStatus.color || 'primary',
                order: projectStatus.order || 0,
                isFinal: projectStatus.isFinal || false,
                projectId: projectStatus.projectId || ''
            });
        } else {
            setFormData({
                name: '',
                color: 'primary',
                order: 0,
                isFinal: false,
                projectId: ''
            });
        }
        setError(null);
    }, [projectStatus, visible]);

    const handleInputChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : (type === 'number' ? parseInt(value, 10) || 0 : value)
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

    const colorOptions = [
        { value: 'primary', label: 'Primário (Azul)' },
        { value: 'secondary', label: 'Secundário (Cinza)' },
        { value: 'success', label: 'Sucesso (Verde)' },
        { value: 'danger', label: 'Perigo (Vermelho)' },
        { value: 'warning', label: 'Aviso (Amarelo)' },
        { value: 'info', label: 'Informação (Ciano)' },
        { value: 'dark', label: 'Escuro' },
        { value: 'light', label: 'Claro' }
    ];

    return (
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {projectStatus ? 'Editar Status do Projeto' : 'Novo Status do Projeto'}
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
                        <CFormLabel htmlFor="color">Cor</CFormLabel>
                        <CFormSelect
                            id="color"
                            name="color"
                            value={formData.color}
                            onChange={handleInputChange}
                        >
                            {colorOptions.map(option => (
                                <option key={option.value} value={option.value}>
                                    {option.label}
                                </option>
                            ))}
                        </CFormSelect>
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="order">Ordem</CFormLabel>
                        <CFormInput
                            type="number"
                            id="order"
                            name="order"
                            value={formData.order}
                            onChange={handleInputChange}
                            min="0"
                        />
                    </div>

                    <div className="mb-3">
                        <CFormSwitch
                            id="isFinal"
                            name="isFinal"
                            checked={formData.isFinal}
                            onChange={handleInputChange}
                            label="É status final"
                        />
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="projectId">ID do Projeto</CFormLabel>
                        <CFormInput
                            type="text"
                            id="projectId"
                            name="projectId"
                            value={formData.projectId}
                            onChange={handleInputChange}
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

export default ProjectStatusModal;
