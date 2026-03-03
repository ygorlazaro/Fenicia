import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormSwitch,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProjectSubtaskModal = ({
    visible,
    onClose,
    onSave,
    projectSubtask,
    loading
}) => {
    const [formData, setFormData] = useState({
        title: '',
        isCompleted: false,
        order: 0,
        completedAt: '',
        taskId: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (projectSubtask) {
            setFormData({
                title: projectSubtask.title || '',
                isCompleted: projectSubtask.isCompleted || false,
                order: projectSubtask.order || 0,
                completedAt: projectSubtask.completedAt ? new Date(projectSubtask.completedAt).toISOString().split('T')[0] : '',
                taskId: projectSubtask.taskId || ''
            });
        } else {
            setFormData({
                title: '',
                isCompleted: false,
                order: 0,
                completedAt: '',
                taskId: ''
            });
        }
        setError(null);
    }, [projectSubtask, visible]);

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
        if (!formData.title) {
            setError('Título é obrigatório.');
            return;
        }

        // Prepare data for submission
        const submitData = {
            ...formData,
            completedAt: formData.completedAt ? new Date(formData.completedAt).toISOString() : null
        };

        onSave(submitData);
    };

    return (
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {projectSubtask ? 'Editar Subtarefa do Projeto' : 'Nova Subtarefa do Projeto'}
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
                        <CFormSwitch
                            id="isCompleted"
                            name="isCompleted"
                            checked={formData.isCompleted}
                            onChange={handleInputChange}
                            label="Concluída"
                        />
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
                        <CFormLabel htmlFor="completedAt">Data de Conclusão</CFormLabel>
                        <CFormInput
                            type="date"
                            id="completedAt"
                            name="completedAt"
                            value={formData.completedAt}
                            onChange={handleInputChange}
                        />
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="taskId">ID da Tarefa</CFormLabel>
                        <CFormInput
                            type="text"
                            id="taskId"
                            name="taskId"
                            value={formData.taskId}
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

export default ProjectSubtaskModal;
