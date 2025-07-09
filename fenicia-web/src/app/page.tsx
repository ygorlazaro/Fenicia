'use client'

import React, { useState } from "react";
import { TokenRequest } from "@/types/requests/TokenRequest";
import { NinjaInput } from "@/components/NinjaInput";
import { NinjaButton } from "@/components/NinjaButton";
import Image from "next/image";
import { TokenService } from "@/services/TokenService";
import Link from "next/link";
import { NinjaMessage } from "@/components/NinjaMessage";
import { setToken } from "@/logic/token";
import { useRouter } from "next/navigation";

const tokenService = new TokenService();

export default function Home() {
  const [request, setRequest] = useState<TokenRequest>({
    email: "ygor@ygorlazaro.com",
    password: "Age14rjy",
    cnpj: "23351185000184"
  });
  const [message, setMessage] = useState<string | undefined>();
  const router = useRouter();

  const handleSubmit = () => {
    const submit = (async () => {
      try {
        const token = await tokenService.getToken(request);
        
        setToken(token.accessToken);
        router.push("/dashboard")
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

          <NinjaInput label="Email" value={request.email} type="email" onChange={(value => setRequest({ ...request, email: value }))} />
          <NinjaInput label="Password" value={request.password} type="password" onChange={(value => setRequest({ ...request, password: value }))} />
          <NinjaInput label="CNPJ" value={request.cnpj} onChange={(value => setRequest({ ...request, cnpj: value }))} />
          
          <NinjaButton label="Login" onClick={handleSubmit} />
          
          <Link href="/signup">
            Signup
          </Link>

          <NinjaMessage message={message} />
        </div>
        
      </main>
    </div>
  );
}
