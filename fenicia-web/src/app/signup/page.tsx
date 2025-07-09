'use client'

import { NinjaButton } from "@/components/NinjaButton";
import { NinjaInput } from "@/components/NinjaInput";
import { NinjaMessage } from "@/components/NinjaMessage";
import { SignUpService } from "@/services/SignUpService";
import { SignUpRequest } from "@/types/requests/SignUpRequest";
import Image from "next/image";
import Link from "next/link";
import { redirect, useRouter } from "next/navigation";
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
        <div className="grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20 font-[family-name:var(--font-geist-sans)]">
            <main className="flex flex-col gap-[32px] row-start-2 items-center sm:items-start">

                <div className="flex flex-col gap-4">
                    <Image src="/logo.jpeg" alt="Logo" width={100} height={100} className="mx-auto" />

                    <NinjaInput label="Name" value={request.name}  onChange={(value => setRequest({ ...request, name: value }))} />
                    <NinjaInput label="Email" value={request.email} type="email" onChange={(value => setRequest({ ...request, email: value }))} />
                    <NinjaInput label="Password" value={request.password} type="password" onChange={(value => setRequest({ ...request, password: value }))} />
                    
                    <NinjaInput label="Company Name" value={request.company.name} onChange={(value => setRequest({ ...request, company: { ...request.company, name: value } }))} />
                    <NinjaInput label="CNPJ" value={request.company.cnpj} onChange={(value => setRequest({ ...request, company: { ...request.company, cnpj: value } }))} />

                    <NinjaButton label="SignUp" onClick={handleSubmit} />
                    
                    <Link href="/">Login</Link>

                    <NinjaMessage message={message} />
                </div>

            </main>
        </div>
    );
}
