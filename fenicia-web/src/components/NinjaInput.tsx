interface NinjaProps {
    label: string;
    value: string;
    onChange: (e: string) => void;
    type?: "text" | "email" | "password";
}

export const NinjaInput = ({label, value, onChange, type= "text"}: NinjaProps) => {
    return <div className="flex flex-col">
        <label className="m-0">{label}</label>
        <input type={type} value={value} onChange={(e) => onChange(e.target.value)} className="border border-gray-700 p-0.5 bg-gray-50" />
    </div>
    
}
