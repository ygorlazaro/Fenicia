import { cilSearch, cilSend, cilX } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CButton, CCard, CCardBody, CCardFooter, CCardHeader, CFormInput, CListGroup, CListGroupItem } from "@coreui/react";
import React, { useEffect, useRef, useState } from "react";
import minionAvatar from "../assets/images/minions.jpeg";
import faqData from "../data/chat-faq.json";

interface ChatMessage {
    id: number;
    sender: "user" | "bot";
    text: string;
    timestamp: Date;
}

interface FaqItem {
    id: number;
    category: string;
    question: string;
    answer: string;
}

const mockMessages: ChatMessage[] = [
    {
        id: 1,
        sender: "bot",
        text: "Olá! Como posso ajudar você hoje? Digite sua dúvida ou escolha uma pergunta frequente abaixo.",
        timestamp: new Date(Date.now() - 1000 * 60 * 5)
    }
];

const findFaqAnswer = (userText: string): string | null => {
    const text = userText.toLowerCase();
    const faqs: FaqItem[] = faqData.faq;

    // Exact match first
    const exactMatch = faqs.find((f) => f.question.toLowerCase() === text);
    if (exactMatch) return exactMatch.answer;

    // Keyword match
    const keywords = text.split(/\s+/).filter((w) => w.length > 2);
    let bestMatch: FaqItem | null = null;
    let bestScore = 0;

    for (const faq of faqs) {
        const qWords = faq.question.toLowerCase().split(/\s+/);
        const aWords = faq.answer.toLowerCase().split(/\s+/);
        let score = 0;

        for (const kw of keywords) {
            if (qWords.some((w) => w.includes(kw))) score += 3;
            if (aWords.some((w) => w.includes(kw))) score += 1;
            if (faq.category.toLowerCase().includes(kw)) score += 2;
        }

        if (score > bestScore) {
            bestScore = score;
            bestMatch = faq;
        }
    }

    return bestScore >= 3 && bestMatch ? bestMatch.answer : null;
};

const ChatWidget: React.FC = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [messages, setMessages] = useState<ChatMessage[]>(mockMessages);
    const [inputValue, setInputValue] = useState("");
    const [showFaq, setShowFaq] = useState(true);
    const messagesEndRef = useRef<HTMLDivElement>(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages, isOpen]);

    const handleToggle = () => {
        setIsOpen(!isOpen);
    };

    const sendBotMessage = (text: string, delay = 1000) => {
        setTimeout(() => {
            const botResponse: ChatMessage = {
                id: Date.now(),
                sender: "bot",
                text,
                timestamp: new Date()
            };
            setMessages((prev) => [...prev, botResponse]);
        }, delay);
    };

    const handleSend = () => {
        if (!inputValue.trim()) return;

        const userText = inputValue.trim();
        const newMessage: ChatMessage = {
            id: Date.now(),
            sender: "user",
            text: userText,
            timestamp: new Date()
        };

        setMessages((prev) => [...prev, newMessage]);
        setInputValue("");
        setShowFaq(false);

        const faqAnswer = findFaqAnswer(userText);
        if (faqAnswer) {
            sendBotMessage(faqAnswer);
        } else {
            sendBotMessage("Não encontrei uma resposta exata para sua pergunta. Nossa equipe de suporte entrará em contato em breve. Enquanto isso, você pode tentar reformular sua dúvida ou escolher uma das perguntas frequentes.");
        }
    };

    const handleFaqClick = (faq: FaqItem) => {
        const userMessage: ChatMessage = {
            id: Date.now(),
            sender: "user",
            text: faq.question,
            timestamp: new Date()
        };
        setMessages((prev) => [...prev, userMessage]);
        setShowFaq(false);
        sendBotMessage(faq.answer);
    };

    const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === "Enter") {
            handleSend();
        }
    };

    const formatTime = (date: Date) => {
        return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    };

    return (
        <div
            style={{
                position: "fixed",
                bottom: "20px",
                right: "20px",
                zIndex: 1050
            }}
        >
            {isOpen && (
                <CCard
                    style={{
                        width: "350px",
                        height: "500px",
                        marginBottom: "10px",
                        display: "flex",
                        flexDirection: "column",
                        boxShadow: "0 4px 12px rgba(0,0,0,0.15)"
                    }}
                >
                    <CCardHeader className="d-flex justify-content-between align-items-center bg-primary text-white">
                        <div className="d-flex align-items-center gap-2">
                            <img src={minionAvatar} alt="Minion" className="rounded-circle" style={{ width: "36px", height: "36px", objectFit: "cover", border: "2px solid #fff" }} />
                            <strong>Suporte Fenicia</strong>
                        </div>
                        <CButton color="light" size="sm" variant="ghost" onClick={handleToggle} className="text-white">
                            <CIcon icon={cilX} />
                        </CButton>
                    </CCardHeader>
                    <CCardBody className="flex-grow-1 overflow-auto p-3" style={{ backgroundColor: "#f8f9fa" }}>
                        <CListGroup flush>
                            {messages.map((msg) => (
                                <CListGroupItem
                                    key={msg.id}
                                    className="border-0 bg-transparent"
                                    style={{
                                        display: "flex",
                                        justifyContent: msg.sender === "user" ? "flex-end" : "flex-start",
                                        padding: "4px 0"
                                    }}
                                >
                                    <div
                                        style={{
                                            maxWidth: "80%",
                                            padding: "8px 12px",
                                            borderRadius: "12px",
                                            backgroundColor: msg.sender === "user" ? "#0d6efd" : "#e9ecef",
                                            color: msg.sender === "user" ? "#fff" : "#212529",
                                            fontSize: "0.875rem"
                                        }}
                                    >
                                        <div>{msg.text}</div>
                                        <div
                                            style={{
                                                fontSize: "0.7rem",
                                                marginTop: "4px",
                                                opacity: 0.7,
                                                textAlign: msg.sender === "user" ? "left" : "right"
                                            }}
                                        >
                                            {formatTime(msg.timestamp)}
                                        </div>
                                    </div>
                                </CListGroupItem>
                            ))}
                            {showFaq && (
                                <CListGroupItem className="border-0 bg-transparent p-0 mt-2">
                                    <div className="text-muted mb-2" style={{ fontSize: "0.75rem" }}>
                                        <CIcon icon={cilSearch} size="sm" className="me-1" />
                                        Perguntas frequentes:
                                    </div>
                                    <div className="d-flex flex-column gap-1">
                                        {(faqData.faq as FaqItem[]).slice(0, 6).map((faq) => (
                                            <CButton key={faq.id} color="light" size="sm" className="text-start text-wrap" style={{ fontSize: "0.8rem", whiteSpace: "normal" }} onClick={() => handleFaqClick(faq)}>
                                                {faq.question}
                                            </CButton>
                                        ))}
                                    </div>
                                </CListGroupItem>
                            )}
                            <div ref={messagesEndRef} />
                        </CListGroup>
                    </CCardBody>
                    <CCardFooter className="p-2">
                        <div className="d-flex gap-2">
                            <CFormInput type="text" placeholder="Digite sua mensagem..." value={inputValue} onChange={(e) => setInputValue(e.target.value)} onKeyDown={handleKeyPress} size="sm" />
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
                    width: "60px",
                    height: "60px",
                    boxShadow: "0 4px 12px rgba(0,0,0,0.2)",
                    padding: 0,
                    overflow: "hidden"
                }}
            >
                <img src={minionAvatar} alt="Minion" style={{ width: "100%", height: "100%", objectFit: "cover" }} />
            </CButton>
        </div>
    );
};

export default ChatWidget;
