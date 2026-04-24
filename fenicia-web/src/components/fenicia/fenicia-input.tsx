import { CFormInput, CFormLabel } from "@coreui/react";
import { useTranslation } from "react-i18next";

interface FeniciaInputProps {
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onBlur?: (e: React.FocusEvent<HTMLInputElement>) => void;
  value: string | number;
  label: string;
  id: string;
  name?: string;
  required?: boolean;
  placeholder?: string;
  type?: 'text' | 'email' | 'password' | 'number' | 'tel' | 'url' | 'date' | 'datetime-local' | 'month' | 'week' | 'time';
  maxLength?: number;
  minLength?: number;
  min?: number;
  step?: number;
  disabled?: boolean;
}

export function FeniciaInput({
  onChange,
  value,
  label,
  id,
  name,
  required,
  type = 'text',
  onBlur,
  placeholder,
  maxLength,
  minLength,
  min = null,
  step = null,
  disabled = false,
}: FeniciaInputProps) {
  const { t } = useTranslation();

  return <>
    <CFormLabel htmlFor={id}>{t(label)} {required && <span className="text-danger">*</span>}</CFormLabel>
    <CFormInput type={type} id={id} name={name ?? id} value={value} onChange={onChange} onBlur={onBlur} required={required} placeholder={placeholder} maxLength={maxLength} minLength={minLength} min={min} step={step} disabled={disabled} />
  </>;
}
