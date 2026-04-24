import { cilChatBubble, cilSend, cilX } from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
    CButton,
    CCard,
    CCardBody,
    CCardFooter,
    CCardHeader,
    CFormInput,
    CListGroup,
    CListGroupItem,
} from '@coreui/react';
import React, { useEffect, useRef, useState } from 'react';

interface ChatMessage {
  id: number;
  sender: 'user' | 'bot';
  text: string;
  timestamp: Date;
}

const mockMessages: ChatMessage[] = [
  {
    id: 1,
    sender: 'bot',
    text: 'Olá! Como posso ajudar você hoje?',
    timestamp: new Date(Date.now() - 1000 * 60 * 5),
  },
  {
    id: 2,
    sender: 'user',
    text: 'Quero saber mais sobre o sistema.',
    timestamp: new Date(Date.now() - 1000 * 60 * 4),
  },
  {
    id: 3,
    sender: 'bot',
    text: 'Claro! O Fenicia é um ERP completo para gestão empresarial. Posso te ajudar com dúvidas sobre módulos, assinaturas ou suporte técnico.',
    timestamp: new Date(Date.now() - 1000 * 60 * 3),
  },
];

const ChatWidget: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState<ChatMessage[]>(mockMessages);
  const [inputValue, setInputValue] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, isOpen]);

  const handleToggle = () => {
    setIsOpen(!isOpen);
  };

  const handleSend = () => {
    if (!inputValue.trim()) return;

    const newMessage: ChatMessage = {
      id: messages.length + 1,
      sender: 'user',
      text: inputValue.trim(),
      timestamp: new Date(),
    };

    setMessages((prev) => [...prev, newMessage]);
    setInputValue('');

    // Mock bot response
    setTimeout(() => {
      const botResponse: ChatMessage = {
        id: messages.length + 2,
        sender: 'bot',
        text: 'Obrigado pela mensagem! Nossa equipe de suporte entrará em contato em breve.',
        timestamp: new Date(),
      };
      setMessages((prev) => [...prev, botResponse]);
    }, 1000);
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      handleSend();
    }
  };

  const formatTime = (date: Date) => {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };

  return (
    <div
      style={{
        position: 'fixed',
        bottom: '20px',
        right: '20px',
        zIndex: 1050,
      }}
    >
      {isOpen && (
        <CCard
          style={{
            width: '350px',
            height: '500px',
            marginBottom: '10px',
            display: 'flex',
            flexDirection: 'column',
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
          }}
        >
          <CCardHeader className="d-flex justify-content-between align-items-center bg-primary text-white">
            <div className="d-flex align-items-center gap-2">
              <div className="rounded-circle bg-light d-flex align-items-center justify-content-center" style={{ width: '32px', height: '32px' }}>
                <CIcon icon={cilChatBubble} size="sm" />
              </div>
              <strong>Suporte Fenicia</strong>
            </div>
            <CButton
              color="light"
              size="sm"
              variant="ghost"
              onClick={handleToggle}
              className="text-white"
            >
              <CIcon icon={cilX} />
            </CButton>
          </CCardHeader>
          <CCardBody className="flex-grow-1 overflow-auto p-3" style={{ backgroundColor: '#f8f9fa' }}>
            <CListGroup flush>
              {messages.map((msg) => (
                <CListGroupItem
                  key={msg.id}
                  className="border-0 bg-transparent"
                  style={{
                    display: 'flex',
                    justifyContent: msg.sender === 'user' ? 'flex-end' : 'flex-start',
                    padding: '4px 0',
                  }}
                >
                  <div
                    style={{
                      maxWidth: '80%',
                      padding: '8px 12px',
                      borderRadius: '12px',
                      backgroundColor: msg.sender === 'user' ? '#0d6efd' : '#e9ecef',
                      color: msg.sender === 'user' ? '#fff' : '#212529',
                      fontSize: '0.875rem',
                    }}
                  >
                    <div>{msg.text}</div>
                    <div
                      style={{
                        fontSize: '0.7rem',
                        marginTop: '4px',
                        opacity: 0.7,
                        textAlign: msg.sender === 'user' ? 'left' : 'right',
                      }}
                    >
                      {formatTime(msg.timestamp)}
                    </div>
                  </div>
                </CListGroupItem>
              ))}
              <div ref={messagesEndRef} />
            </CListGroup>
          </CCardBody>
          <CCardFooter className="p-2">
            <div className="d-flex gap-2">
              <CFormInput
                type="text"
                placeholder="Digite sua mensagem..."
                value={inputValue}
                onChange={(e) => setInputValue(e.target.value)}
                onKeyDown={handleKeyPress}
                size="sm"
              />
              <CButton color="primary" size="sm" onClick={handleSend}>
                <CIcon icon={cilSend} />
              </CButton>
            </div>
          </CCardFooter>
        </CCard>
      )}

      <CButton
        color="primary"
        shape="rounded-circle"
        size="lg"
        onClick={handleToggle}
        style={{
          width: '60px',
          height: '60px',
          boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
        }}
      >
        <CIcon icon={cilChatBubble} size="lg" />
      </CButton>
    </div>
  );
};

export default ChatWidget;
