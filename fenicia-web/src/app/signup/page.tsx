'use client'

import { NinjaAnonymousForm } from "@/components/NinjaAnonymousForm";
import { NinjaButton } from "@/components/NinjaButton";
import { NinjaInput } from "@/components/NinjaInput";
import { NinjaLink } from "@/components/NinjaLink";
import { NinjaMessage } from "@/components/NinjaMessage";
import { SignUpService } from "@/services/SignUpService";
import { SignUpRequest } from "@/types/requests/SignUpRequest";
import { useRouter } from "next/navigation";
import { useState } from "react";

const signUpService = new SignUpService();

export default function Home() {
    const [request, setRequest] = useState<SignUpRequest>({
        name: "Ygor Lazaro",
        email: "ygor@ygorlazaro.com",
        password: "Age14rjy",
        company: {
            name: "Gato Ninja",
            cnpj: "23351185000184"
        }
    });
    const [message, setMessage] = useState<string | undefined>();

    const router = useRouter();

    const handleSubmit = () => {
        const submit = (async () => {
            try {
                await signUpService.signUp(request);
                
                router.push("/");
            } catch (error: any) {
                setMessage(error.response?.data.message);
            }
        })

        submit();
    }

    return (
        <NinjaAnonymousForm>
            <NinjaInput label="Name" value={request.name} onChange={(value => setRequest({ ...request, name: value }))} />
            <NinjaInput label="Email" value={request.email} type="email" onChange={(value => setRequest({ ...request, email: value }))} />
            <NinjaInput label="Password" value={request.password} type="password" onChange={(value => setRequest({ ...request, password: value }))} />

            <NinjaInput label="Company Name" value={request.company.name} onChange={(value => setRequest({ ...request, company: { ...request.company, name: value } }))} />
            <NinjaInput label="CNPJ" value={request.company.cnpj} onChange={(value => setRequest({ ...request, company: { ...request.company, cnpj: value } }))} />

            <NinjaButton label="SignUp" onClick={handleSubmit} />

            <NinjaLink label="Login" href="/" className="self-end" />

            <NinjaMessage message={message} />
        </NinjaAnonymousForm>
    );
}
