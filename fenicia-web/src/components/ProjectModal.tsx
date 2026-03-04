import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormTextarea,
    CFormSelect,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProjectModal = ({
    visible,
    onClose,
    onSave,
    project,
    loading
}) => {
    const [formData, setFormData] = useState({
        title: '',
        description: '',
        status: 'Active',
        startDate: '',
        endDate: '',
        owner: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (project) {
            setFormData({
                title: project.title || '',
                description: project.description || '',
                status: project.status || 'Active',
                startDate: project.startDate ? new Date(project.startDate).toISOString().split('T')[0] : '',
                endDate: project.endDate ? new Date(project.endDate).toISOString().split('T')[0] : '',
                owner: project.owner || ''
            });
        } else {
            setFormData({
                title: '',
                description: '',
                status: 'Active',
                startDate: '',
                endDate: '',
                owner: ''
            });
        }
        setError(null);
    }, [project, visible]);

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

        if (!formData.title) {
            setError('Título é obrigatório.');
            return;
        }

        if (!formData.owner) {
            setError('Proprietário é obrigatório.');
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
                    {project ? 'Editar Projeto' : 'Novo Projeto'}
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
                        <CFormLabel htmlFor="title">Título *</CFormLabel>
                        <CFormInput
                            type="text"
                            id="title"
                            name="title"
                            value={formData.title}
                            onChange={handleInputChange}
                            required
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

                    <div className="mb-3">
                        <CFormLabel htmlFor="status">Status *</CFormLabel>
                        <CFormSelect
                            id="status"
                            name="status"
                            value={formData.status}
                            onChange={handleInputChange}
                            required
                        >
                            <option value="Draft">Rascunho</option>
                            <option value="Active">Ativo</option>
                            <option value="Archived">Arquivado</option>
                            <option value="Completed">Concluído</option>
                        </CFormSelect>
                    </div>

                    <CRow>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="startDate">Data de Início</CFormLabel>
                                <CFormInput
                                    type="date"
                                    id="startDate"
                                    name="startDate"
                                    value={formData.startDate}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                        <CCol md={6}>
                            <div className="mb-3">
                                <CFormLabel htmlFor="endDate">Data de Fim</CFormLabel>
                                <CFormInput
                                    type="date"
                                    id="endDate"
                                    name="endDate"
                                    value={formData.endDate}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </CCol>
                    </CRow>

                    <div className="mb-3">
                        <CFormLabel htmlFor="owner">ID do Proprietário *</CFormLabel>
                        <CFormInput
                            type="text"
                            id="owner"
                            name="owner"
                            value={formData.owner}
                            onChange={handleInputChange}
                            placeholder="UUID do proprietário"
                            required
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

export default ProjectModal;
