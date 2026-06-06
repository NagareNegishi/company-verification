# MBIE API Access Agreement — Compliance Checklist

**Source:** MBIE API Access Agreement (Version: November 2022)
**For:** NZBN Sandbox API (open-source, non-commercial app)
**Acronyms:** MBIE (Ministry of Business, Innovation & Employment), API (Application Programming Interface), NZBN (New Zealand Business Number), PPSR (Personal Property Securities Register)

> Not legal advice. This is a plain-language summary to help you track obligations. Review clauses 13–15 yourself before signing.

---

## 1. Before you start (the thing you flagged)

- [ ] **Provide a signed MBIE API Access Agreement.** New users must submit this during the subscription approval process — it is requested if not already on file. *(This is the document MBIE's email referred to.)*
- [ ] Complete **page 1**: date + your name and address in the "AND" block.
- [ ] Complete **page 9** ("EXECUTED" section, second block): signature, print name, email, phone.
- [ ] Keep a copy of the signed agreement for your records.

---

## 2. Attribution (easy to forget — build it into the app)

- [ ] Display an acknowledgement that **MBIE is the source** of any data shown. *(Clause 4.8)*
- [ ] Include with each data set *(Schedule 2)*:
  - [ ] Date and time of the search
  - [ ] Source text in the format: **"Data sourced from the [Register name]"**

---

## 3. How you may use the data

- [ ] Only display data you specifically selected to receive; **no ownership transfers** to you — it's a limited licence. *(Clause 4.1)*
- [ ] Use data **lawfully**, including under the **Privacy Act 2020** and the legislation governing each register. *(Clause 4.3)*
- [ ] Do **not** display data on any **Excluded Website** *(Clause 4.4, Schedule 1)* — sites that:
  - incite hatred / anti-social behaviour
  - promote violence or terrorism
  - discriminate against or exploit vulnerable groups
  - facilitate illegal activity
  - are misleading, pornographic, defamatory, or otherwise unlawful
  - infringe individual privacy
- [ ] If you charge anyone for data that's free from the registers, the fee must be **reasonable**. *(Clause 4.7 — likely N/A for a free open-source app)*
- [ ] If you give data to third parties (at your own risk), use **best endeavours** to make them follow these same terms, and allow MBIE to enforce them directly. *(Clauses 4.2, 4.11)*

---

## 4. Things you must NOT do

- [ ] Do **not** reverse-engineer, decompile, modify, or make derivative works of the API. *(Clause 4.10a)* — **Note for open-source:** this restricts the *API itself*, not your own app code. Keep your repo clear of MBIE's API internals/specs.
- [ ] Do **not** remove copyright / trademark / proprietary notices. *(Clause 4.10b)*
- [ ] Do **not** offer/resell/sub-licence the API to others. *(Clause 4.10c)*
- [ ] Do **not** use MBIE trademarks or logos. *(Clause 11.2)*
- [ ] Do **not** engage in illegal or offensive behaviour using the API or data. *(Clause 6.3)*

---

## 5. Security & your logon

- [ ] Keep your **API logon safe**; never share it — it's for named individuals in your org only. *(Clauses 7.7, 7.7)*
- [ ] Maintain a **list of authorised individuals** who can access the API. *(Clause 7.5b)*
- [ ] Prevent unauthorised use and maintain security measures to best industry standards. *(Clauses 7.1, 7.4)*
- [ ] Do **not** attempt to bypass security mechanisms or gain unauthorised access. *(Clauses 7.5c, 7.5d)*
- [ ] Use best endeavours to stop viruses/malware reaching MBIE systems. *(Clause 7.5a)*
- [ ] **Notify MBIE immediately** of any unauthorised use, security violation, third-party claim, or disclosure of your RealMe login — and cease use if MBIE requests. *(Clauses 7.1c, 7.1d, 7.1e, 7.8)*

---

## 6. Technical & operational

- [ ] **Build your own software** — MBIE supplies data, not code. *(Clause 1.2)*
- [ ] Ensure correct hardware/software config and do **full systems testing**. *(Clause 6.2)*
- [ ] Comply with technical/organisational requirements in the **API Specifications**. *(Clause 6.5)*
- [ ] Keep your **API account details up to date**. *(Clause 6.5)*
- [ ] **Report faults, errors, or omissions** in the data or API directly to MBIE. *(Clause 6.1)*

---

## 7. Ongoing / awareness

- [ ] Respond within **3 working days** to any MBIE compliance request; expect possible audit/tracking of your use. *(Clause 4.9)*
- [ ] Watch for **agreement changes** — MBIE gives 10 working days' notice via the API website/email; continued use = acceptance. *(Clauses 2, 12)*
- [ ] Make no **misleading or deceptive statements** about MBIE, the data, or the API. *(Clause 14.1a)*

---

## 8. Risk you're accepting (know these, no action needed)

- You **indemnify MBIE** against losses/claims arising from your use. *(Clause 13)*
- Service is **"as is", no warranties**; MBIE excludes liability to the max extent of law. *(Clauses 14.2, 15)*
- For business use, **Consumer Guarantees Act 1993 does not apply**. *(Clause 15.4)*

---

## 9. If it ends

- [ ] On termination: **stop using the API** and **delete all MBIE data and API functionality** from your systems and site. *(Clause 18.2)*
- Either party may terminate immediately with written notice. *(Clause 17.2)*
- If you breach: you get **15 days** to remedy before MBIE can terminate. *(Clause 17.5)*

---

## 10. Probably not applicable to you

- [ ] **PPSR / Motor Vehicle Register terms (Clause 20)** — only applies if you use the PPSR API. An NZBN-only app is not affected.
- [ ] **Insolvency Register time limits (Clauses 4.5–4.6)** — only relevant if you display insolvency data.
