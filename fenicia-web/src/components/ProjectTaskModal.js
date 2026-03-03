import React, { useEffect, useState } from 'react';
import {
    CButton,
    CForm,
    CFormInput,
    CFormLabel,
    CFormSelect,
    CFormTextarea,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CAlert
} from '@coreui/react';

const ProjectTaskModal = ({
    visible,
    onClose,
    onSave,
    projectTask,
    loading
}) => {
    const [formData, setFormData] = useState({
        title: '',
        description: '',
        priority: 'Medium',
        type: 'Task',
        order: 0,
        estimatePoints: '',
        dueDate: '',
        projectId: '',
        statusId: '',
        createdBy: ''
    });
    const [error, setError] = useState(null);

    useEffect(() => {
        if (projectTask) {
            setFormData({
                title: projectTask.title || '',
                description: projectTask.description || '',
                priority: projectTask.priority || 'Medium',
                type: projectTask.type || 'Task',
                order: projectTask.order || 0,
                estimatePoints: projectTask.estimatePoints?.toString() || '',
                dueDate: projectTask.dueDate ? new Date(projectTask.dueDate).toISOString().split('T')[0] : '',
                projectId: projectTask.projectId || '',
                statusId: projectTask.statusId || '',
                createdBy: projectTask.createdBy || ''
            });
        } else {
            setFormData({
                title: '',
                description: '',
                priority: 'Medium',
                type: 'Task',
                order: 0,
                estimatePoints: '',
                dueDate: '',
                projectId: '',
                statusId: '',
                createdBy: ''
            });
        }
        setError(null);
    }, [projectTask, visible]);

    const handleInputChange = (e) => {
        const { name, value, type } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'number' ? (value ? parseInt(value, 10) : '') : value
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
            estimatePoints: formData.estimatePoints ? parseInt(formData.estimatePoints, 10) : null,
            order: formData.order || 0
        };

        onSave(submitData);
    };

    const priorityOptions = [
        { value: 'Low', label: 'Baixa' },
        { value: 'Medium', label: 'Média' },
        { value: 'High', label: 'Alta' },
        { value: 'Critical', label: 'Crítica' }
    ];

    const typeOptions = [
        { value: 'Task', label: 'Tarefa' },
        { value: 'Bug', label: 'Bug' },
        { value: 'Feature', label: 'Funcionalidade' },
        { value: 'Improvement', label: 'Melhoria' },
        { value: 'Spike', label: 'Spike' }
    ];

    return (
        <CModal
            visible={visible}
            onClose={onClose}
            size="lg"
        >
            <CModalHeader>
                <CModalTitle>
                    {projectTask ? 'Editar Tarefa do Projeto' : 'Nova Tarefa do Projeto'}
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

                    <div className="row">
                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="priority">Prioridade</CFormLabel>
                            <CFormSelect
                                id="priority"
                                name="priority"
                                value={formData.priority}
                                onChange={handleInputChange}
                            >
                                {priorityOptions.map(option => (
                                    <option key={option.value} value={option.value}>
                                        {option.label}
                                    </option>
                                ))}
                            </CFormSelect>
                        </div>

                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="type">Tipo</CFormLabel>
                            <CFormSelect
                                id="type"
                                name="type"
                                value={formData.type}
                                onChange={handleInputChange}
                            >
                                {typeOptions.map(option => (
                                    <option key={option.value} value={option.value}>
                                        {option.label}
                                    </option>
                                ))}
                            </CFormSelect>
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md-6 mb-3">
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

                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="estimatePoints">Pontos de Estimativa</CFormLabel>
                            <CFormInput
                                type="number"
                                id="estimatePoints"
                                name="estimatePoints"
                                value={formData.estimatePoints}
                                onChange={handleInputChange}
                                min="0"
                            />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="dueDate">Data de Vencimento</CFormLabel>
                            <CFormInput
                                type="date"
                                id="dueDate"
                                name="dueDate"
                                value={formData.dueDate}
                                onChange={handleInputChange}
                            />
                        </div>

                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="projectId">ID do Projeto</CFormLabel>
                            <CFormInput
                                type="text"
                                id="projectId"
                                name="projectId"
                                value={formData.projectId}
                                onChange={handleInputChange}
                            />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="statusId">ID do Status</CFormLabel>
                            <CFormInput
                                type="text"
                                id="statusId"
                                name="statusId"
                                value={formData.statusId}
                                onChange={handleInputChange}
                            />
                        </div>

                        <div className="col-md-6 mb-3">
                            <CFormLabel htmlFor="createdBy">Criado Por</CFormLabel>
                            <CFormInput
                                type="text"
                                id="createdBy"
                                name="createdBy"
                                value={formData.createdBy}
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

export default ProjectTaskModal;
