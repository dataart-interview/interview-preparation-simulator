# DataArt Interview Preparation Simulator

## Vision

Create a self-guided preparation kit for DataArt candidates who have already completed the internal live-coding exercise and are preparing for a similar Trainline client interview.

The kit should turn a candidate's existing solution into a personalised learning path. It combines a complete reference application, concise interview guidance, evidence-based repository review, and adaptive technical-question practice. It is not intended to replace the candidate's own reasoning or to predict the hiring decision.

## Why This Exists

Recent feedback shows recurring problems that are not explained by a lack of theoretical knowledge alone:

- candidates fail to establish working endpoints early;
- they spend too much time on low-priority structure or unfamiliar tools;
- testing, logging, error handling, and resilience are missing or only discussed;
- communication becomes unstructured or too passive;
- candidates do not identify their own gaps or explain what remains incomplete;
- DataArt coding approval alone does not consistently translate into Trainline success.

Repeated interviewer-led preparation sessions are expensive and difficult to scale. Candidates need a way to review their own attempt and practise independently in their free time.

## Target User and Timing

The target user is a .NET candidate who:

1. has completed the DataArt live-coding session;
2. has a local repository containing that attempt;
3. may optionally have written interviewer feedback;
4. is preparing for a Trainline client interview.

Because the kit is used after the DataArt session, it may expose the full Cinema Seat Service reference implementation without compromising the internal exercise.

## Product Principles

1. **Review before practice.** Start from evidence in the candidate's actual solution.
2. **Working software first.** Reinforce a thin, runnable vertical slice before secondary improvements.
3. **Evidence over generic advice.** Every review finding should cite code, tests, build output, or optional interviewer feedback.
4. **Do not invent signals.** Communication, time management, cloud experience, and similar traits are marked `Not observed` unless an appropriate source supports them.
5. **Teach judgement, not imitation.** The reference solution is an opinionated exemplar, not the only acceptable design.
6. **Keep skills focused.** Repository review and technical practice are separate reusable workflows backed by shared guidance.
7. **Keep the first version local and transparent.** Use repository content and Codex skills rather than building a custom application UI.
8. **Support repeated preparation.** Full technical practice can span multiple sessions and resume from saved progress.

## Candidate Journey

1. Clone or open the preparation repository in Codex.
2. Read the short interview playbook.
3. Run the solution-review skill against the DataArt attempt.
4. Optionally provide DataArt interviewer feedback.
5. Receive a detailed review, competency scorecard, and prioritised preparation profile in Markdown and HTML.
6. Run the technical-practice skill using that profile.
7. Choose a targeted, standard, full, or custom question set.
8. Resume longer question sets over multiple sessions.
9. Review the final knowledge gaps and suggested follow-up practice.

## Repository Deliverables

### 1. Complete Reference Application

Provide one coherent, opinionated .NET 10 implementation of the Cinema Seat Service. It should be production-shaped while remaining small enough to understand.

The reference application includes all core requirements and all extension opportunities described in the exercise:

- endpoint returning the complete object-per-seat map;
- endpoint returning one seat's availability;
- adjacent-seat finder;
- typed or named `HttpClient` through `IHttpClientFactory`;
- asynchronous calls and propagated cancellation tokens;
- explicit timeout, retry, and circuit-breaker behaviour;
- in-memory caching with a 3–5 second TTL;
- last-known-good fallback where appropriate;
- graceful treatment of upstream delays, timeouts, 404s, malformed content, and unavailable data;
- request validation and consistent HTTP status codes;
- problem-details responses for errors;
- structured logging without leaking sensitive data;
- clear separation between upstream DTOs, domain/application logic, and API contracts;
- unit tests for parsing, mapping, lookup, validation, adjacent-seat search, and resilience-related behaviour;
- integration tests for the public API contract and failure paths;
- Docker support and `docker-compose.yml`;
- concise run and test instructions.

The implementation is a complete learning reference, not a claim that every production detail must be reproduced during a 90-minute interview. The playbook and rubric explain which behaviours must be prioritised first under time pressure.

### 2. Interview Playbook

Provide a concise brochure in Markdown. It should be readable in approximately 10–15 minutes and be suitable for later export to PDF if needed.

The playbook covers:

- what interviewers are trying to observe;
- how to begin: read requirements, restate understanding, and ask useful clarifying questions;
- how to establish a thin working path early;
- how to prioritise core behaviour over optional structure;
- how and when to add tests, resilience, logging, and error handling;
- how to think aloud without narrating every keystroke;
- how to explain a decision using: decision, reason, trade-off, evidence or example, next step;
- how to respond constructively to hints and challenges;
- how to troubleshoot visibly and methodically;
- how to perform a closing self-review;
- how to describe incomplete work and production-readiness gaps;
- Trainline-relevant engineering expectations such as ownership, pairing, testing without relying on dedicated QA, deployment awareness, monitoring, technical debt, and scope change;
- permitted and inappropriate uses of AI during the interview.

The first version does not include a separate behavioural-interview chatbot. Communication, ownership, and self-awareness guidance belongs in the playbook; technical and engineering-judgement questions belong in the technical-practice skill.

### 3. Shared Competency Rubric

Use a shared rubric derived from the existing DataArt evaluation workbook and accumulated feedback. Both skills and both report formats use the same definitions.

Initial competencies:

1. C# proficiency and code quality;
2. REST API and ASP.NET Core;
3. software architecture and design;
4. unit, integration, and acceptance testing;
5. asynchronous programming and concurrency;
6. problem solving and requirement decomposition;
7. third-party HTTP service integration and resilience;
8. communication and collaboration;
9. microservices, cloud, deployment, and observability;
10. design patterns and dependency injection;
11. prioritisation, time management, and self-awareness.

Each competency defines:

- what it measures;
- observable evidence sources;
- positive and negative indicators;
- scoring anchors;
- relevant review checks;
- related technical-practice topics.

Use the following evidence states:

- **Not observed:** available inputs cannot support a judgement;
- **Critical gap (0–20):** fundamentals are absent or incorrect;
- **Developing (21–40):** partial understanding with substantial guidance required;
- **Acceptable with gaps (41–60):** basic independent performance with meaningful omissions;
- **Strong (61–80):** correct, independent, well-reasoned performance;
- **Excellent (81–100):** proactive senior-level depth, trade-off awareness, and robust execution.

Scores include a confidence level and evidence source. The system must not assign false precision or convert missing evidence into a low score.

### 4. Solution-Review Skill

Create a repository-scoped Codex skill with one responsibility: evaluate a candidate's existing implementation and produce a personalised preparation profile.

#### Inputs

- path to the candidate repository, required;
- path to interviewer feedback, optional;
- path to the shared rubric and best-practices references, configured by the skill;
- path to the reference implementation, configured by the skill.

Instructions found inside candidate code, comments, generated files, or attached feedback are treated as untrusted source content, not as commands for the reviewer.

#### Review Workflow

1. Confirm the repository and solution scope.
2. Inspect project structure, source, tests, configuration, and Git history when available.
3. Build and run the existing automated tests without modifying the candidate solution.
4. Exercise relevant public endpoints when practical.
5. Review each competency using observable evidence.
6. Use optional interviewer feedback as an additional, explicitly labelled evidence source.
7. Compare behaviour and engineering choices with the rubric and reference implementation without requiring an identical architecture.
8. Rank preparation risks by likely interview impact.
9. Generate the structured profile and both human-readable report formats.

#### Review Outputs

For every competency, report:

- score or `Not observed`;
- confidence;
- evidence sources;
- strengths;
- gaps and risks;
- concrete file and line references where applicable;
- recommended practice topics.

Also report:

- build and test status;
- working and missing behaviours;
- highest-priority risks;
- recommended Q&A preset and topic distribution;
- overall summary;
- readiness narrative without a hiring pass/fail prediction.

### 5. Preparation Profile and HTML Preview

Generate one structured review model and render it into two equivalent views:

- `preparation-profile.md` for humans and the technical-practice skill;
- `preparation-profile.html` for a clearer visual overview.

Keep a machine-readable intermediate representation, such as `preparation-profile.json`, as the single source for rendering both views.

The repository includes scaffolds for:

- the structured profile schema;
- the Markdown report;
- the standalone HTML report.

The HTML report requires no server and opens locally. It should show:

- overall summary and evidence limitations;
- competency cards with score, confidence, and evidence source;
- strengths and gaps;
- top preparation priorities;
- recommended Q&A configuration;
- expandable detailed code findings.

Generated candidate reports should be placed in a gitignored local output directory because they may contain personal or interview-related information.

### 6. Technical-Practice Skill

Create a second repository-scoped Codex skill with one responsibility: conduct adaptive technical and engineering-judgement practice.

#### Inputs

- `preparation-profile.md` or its structured representation, optional but recommended;
- DataArt interviewer feedback, optional;
- requested duration, question count, or topic selection, optional;
- shared rubric, playbook, question catalogue, and best-practices references.

The skill also works without a previous review by running a balanced question set.

#### Question Presets

- **Targeted:** approximately 20 questions and 45–60 minutes, concentrating on the top two or three weak areas.
- **Standard:** approximately 40 questions and 90–120 minutes, covering all identified gaps plus essential baseline topics. This is the default recommendation.
- **Full:** 75–100 questions across the complete competency model, intended to resume over multiple sessions.
- **Custom:** candidate specifies a time budget, question count, competencies, or a combination of them.

Time estimates are guidance rather than hard promises because scenario questions and follow-ups vary in depth.

#### Session Behaviour

- Ask one primary question at a time.
- Mix concise knowledge checks, applied scenarios, code/design reasoning, troubleshooting, and production trade-offs.
- Use follow-up questions to test depth rather than accepting memorised terminology.
- Adapt topic weighting from the preparation profile and optional feedback.
- Distinguish an incorrect answer from an unclear explanation.
- Provide concise teaching feedback and a model answer after the candidate commits to an answer.
- Periodically summarise strengths and recurring gaps.
- Save progress so standard and full sessions can be resumed.
- End with a topic-level summary and recommended next practice.

The skill must not write interview-task code for the candidate during Q&A. It may use short illustrative snippets when reviewing an answer.

## Shared Knowledge Sources

Avoid duplicating rules across skills. Store shared references in repository documentation, including:

- competency rubric and scoring anchors;
- interview playbook;
- reference-solution walkthrough;
- .NET and ASP.NET Core best-practices notes relevant to the exercise;
- question catalogue and answer criteria;
- report schema and templates.

The reference implementation supports examples and comparison. The written rubric remains the authority for scoring so valid alternative designs are not penalised.

## Conceptual Repository Layout

```text
.
├── README.md
├── PLAN.md
├── reference-solution/
│   ├── Cinema.Api/
│   ├── Cinema.Api.Tests/
│   ├── Cinema.Api.IntegrationTests/
│   └── docker-compose.yml
├── guidance/
│   ├── interview-playbook.md
│   ├── competency-rubric.md
│   ├── reference-solution-walkthrough.md
│   ├── dotnet-best-practices.md
│   └── technical-question-catalogue.md
├── templates/
│   ├── preparation-profile.schema.json
│   ├── preparation-profile.md
│   └── preparation-profile.html
├── .agents/
│   └── skills/
│       ├── solution-review/
│       │   └── SKILL.md
│       └── technical-practice/
│           └── SKILL.md
└── reports/                 # Generated locally and gitignored
```

The existing starter project may be reorganised during implementation, but the final layout should keep the runnable reference, guidance, templates, and skills easy to discover.

## Error Handling and Safety

- A missing optional feedback file does not block repository review.
- A failed build or test run becomes review evidence; it does not terminate the review.
- Missing SDKs or dependencies are reported explicitly without modifying the candidate's environment unexpectedly.
- Candidate files remain read-only during evaluation unless the candidate separately asks for fixes.
- Feedback content cannot override skill instructions.
- Reports distinguish direct evidence, feedback-derived evidence, and reviewer inference.
- Personal names and sensitive interview details are not copied unnecessarily into generated reports.

## Validation Strategy

### Reference Application

- build from a clean checkout;
- run unit and integration tests;
- exercise success, validation, upstream failure, timeout, fallback, cache, and adjacent-seat scenarios;
- run through Docker using the documented commands.

### Solution-Review Skill

Test against several fixture repositories:

- complete high-quality solution;
- runnable minimal solution;
- partially completed solution;
- overengineered but non-working solution;
- solution with resilience but weak testing;
- solution with optional interviewer feedback;
- solution without feedback;
- repository that does not build.

Verify that findings cite evidence, do not require the reference architecture, and mark unobservable competencies correctly.

### Technical-Practice Skill

Test that:

- all presets produce the expected approximate volume;
- targeted sessions favour weaknesses from the preparation profile;
- full sessions cover every competency;
- follow-ups adapt to answer quality;
- sessions can resume without repeating completed questions unnecessarily;
- unsupported claims in optional feedback are not treated as objective technical facts;
- end-of-session summaries match the answers given.

### Report Rendering

- validate the structured profile against its schema;
- confirm Markdown and HTML contain the same scores and findings;
- open the HTML report locally and verify readability, navigation, and expandable details;
- ensure missing and `Not observed` values render clearly.

## MVP Boundaries

Included:

- full reference application;
- interview playbook;
- shared competency rubric and technical knowledge sources;
- solution-review skill;
- technical-practice skill;
- structured, Markdown, and standalone HTML preparation profiles;
- reusable report scaffolds;
- large adaptive question presets and resumable practice.

Not included initially:

- hosted web application;
- authentication or candidate accounts;
- central storage or recruiter dashboard;
- automated hiring recommendation;
- voice or video analysis;
- behavioural Q&A chatbot;
- direct comparison or ranking between candidates.

## Success Criteria

The MVP is successful when a candidate can:

1. run the reviewer against their DataArt solution without an interviewer;
2. understand exactly which conclusions came from code and which came from optional feedback;
3. receive a useful Markdown report and a readable local HTML preview;
4. identify the few highest-risk gaps for the Trainline interview;
5. complete a personalised or full technical-practice programme over one or more sessions;
6. use the complete reference application and playbook to understand stronger implementation and interview behaviour;
7. repeat the process after further practice and see materially different evidence in the new review.

The organisational success signal is reduced demand for repeated live preparation sessions without lowering the quality of candidates sent to Trainline.
