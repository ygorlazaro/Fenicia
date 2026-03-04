import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormSelect,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProjectTaskAssigneeModal = ({
    visible,
    onClose,
    onSave,
    projectTaskAssignee,
    loading
}) => {
    const [formData, setFormData] = useState({
        taskId: '',
        userId: '',
        role: 'Contributor',
        assignedAt: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (projectTaskAssignee) {
            setFormData({
                taskId: projectTaskAssignee.taskId || '',
                userId: projectTaskAssignee.userId || '',
                role: projectTaskAssignee.role || 'Contributor',
                assignedAt: projectTaskAssignee.assignedAt ? new Date(projectTaskAssignee.assignedAt).toISOString().split('T')[0] : ''
            });
        } else {
            setFormData({
                taskId: '',
                userId: '',
                role: 'Contributor',
                assignedAt: ''
            });
        }
        setError(null);
    }, [projectTaskAssignee, visible]);

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
        if (!formData.taskId) {
            setError('ID da tarefa é obrigatório.');
            return;
        }

        if (!formData.userId) {
            setError('ID do usuário é obrigatório.');
            return;
        }

        // Prepare data for submission
        const submitData = {
            ...formData,
            assignedAt: formData.assignedAt ? new Date(formData.assignedAt).toISOString() : new Date().toISOString()
        };

        onSave(submitData);
    };

    const roleOptions = [
        { value: 'Owner', label: 'Proprietário' },
        { value: 'Contributor', label: 'Contribuidor' },
        { value: 'Reviewer', label: 'Revisor' }
    ];

    return (
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {projectTaskAssignee ? 'Editar Responsável da Tarefa' : 'Novo Responsável da Tarefa'}
                </CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSubmit}>
                <CModalBody>
                    {error && (
                        <CAlert color="danger" dismissible>
                            {error}
                        </CAlert>
                    )}

                    <div className="row">
                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="taskId">ID da Tarefa *</CFormLabel>
                            <CFormInput
                                type="text"
                                id="taskId"
                                name="taskId"
                                value={formData.taskId}
                                onChange={handleInputChange}
                                required
                            />
                        </div>

                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="userId">ID do Usuário *</CFormLabel>
                            <CFormInput
                                type="text"
                                id="userId"
                                name="userId"
                                value={formData.userId}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="role">Função</CFormLabel>
                            <CFormSelect
                                id="role"
                                name="role"
                                value={formData.role}
                                onChange={handleInputChange}
                            >
                                {roleOptions.map(option => (
                                    <option key={option.value} value={option.value}>
                                        {option.label}
                                    </option>
                                ))}
                            </CFormSelect>
                        </div>

                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="assignedAt">Data de Atribuição</CFormLabel>
                            <CFormInput
                                type="date"
                                id="assignedAt"
                                name="assignedAt"
                                value={formData.assignedAt}
                                onChange={handleInputChange}
                            />
                        </div>
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

export default ProjectTaskAssigneeModal;
