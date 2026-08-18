# General Engineering and Interview Best Practices

## Purpose

This guide covers engineering behaviour that is valuable during interviews and day-to-day delivery but is not reliably measurable from a source repository alone. It is intentionally separate from `dotnet-best-practices.md`, which is designed for technical solution evaluation.

Use this guide for interview preparation, self-Q&A, communication practice, and discussion of teamwork, ownership, delivery, debugging, and production responsibility. Do not turn these ideas into conclusions about a candidate from repository code alone; use direct feedback for personalised observations.

## 1. Live-Coding Delivery

### 1.1 Confirm the problem before coding

**Prefer:** Read the complete specification, restate the goal, identify inputs and outputs, and ask a few consequential questions about ambiguous business rules and success criteria.

**Avoid:** Starting from the first familiar keyword, silently inventing requirements, or asking the interviewer to approve every minor implementation detail.

**Why it matters:** Clarification prevents avoidable rework while showing that the engineer can validate understanding and still drive the session independently.

### 1.2 Build a thin vertical slice first

**Prefer:** Implement one complete path from request to dependency, mapping, business logic, response, and execution check before expanding the design.

**Avoid:** Building every project, abstraction, repository, cache, middleware component, and test fixture before any endpoint works.

**Why it matters:** A working slice creates observable evidence quickly and provides a stable base for incremental improvements.

### 1.3 Prioritise required behaviour

**Prefer:** Classify work as must-have, should-have, and production follow-up. Revisit that order as the remaining time changes.

**Avoid:** Spending most of the session on audit fields, generic repositories, distributed infrastructure, documentation polish, or other work the task does not require.

**Why it matters:** Delivery under constraints is part of engineering judgement. The interview feedback repeatedly links rejection to low-value work displacing core functionality.

### 1.4 Time-box design decisions

**Prefer:** Choose a reasonable familiar design, state why it is sufficient, and move forward. Return to reversible refinements after the main path works.

**Avoid:** Debating several architectures, libraries, serializers, or cache strategies without converting the decision into working software.

**Why it matters:** A good reversible decision made promptly is more valuable than an ideal design that prevents delivery.

### 1.5 Work in executable increments

**Prefer:** Compile and run after startup configuration, dependency integration, mapping, the first endpoint, each business rule, and important tests.

**Avoid:** Writing a large amount of unexecuted code and discovering registration, routing, or serialization failures near the end.

**Why it matters:** Short feedback loops localise failures and leave the solution demonstrable if the session ends unexpectedly.

### 1.6 Make shortcuts explicit

**Prefer:** State which shortcut is being taken, why it is acceptable for the exercise, and what would replace it in production.

**Avoid:** Quietly hardcoding values, skipping failure handling, merging responsibilities, or omitting tests and expecting the interviewer to infer that the omissions are temporary.

**Why it matters:** Explicit trade-offs distinguish deliberate prioritisation from an unrecognised knowledge gap.

### 1.7 Keep the environment ready

**Prefer:** Verify the SDK, IDE, terminal, repository access, restore, build, test runner, and screen sharing before the session.

**Avoid:** Installing essential tools, learning the IDE, or resolving predictable environment problems during the exercise.

**Why it matters:** Environment friction consumes time in which the interviewer could otherwise observe engineering ability.

### 1.8 Finish with an honest review

**Prefer:** Demonstrate what works, identify known defects and omissions, and list the next two or three improvements in priority order.

**Avoid:** Claiming production readiness, apologising vaguely, or listing every possible enterprise feature without relating it to the actual solution.

**Why it matters:** Strong engineers define done, expose risk, and turn incomplete work into a credible follow-up plan.

## 2. Debugging and Tool Use

### 2.1 Read the failure before changing code

**Prefer:** Identify the first relevant error, inspect the failing boundary and actual values, form a hypothesis, and make the smallest change that tests it.

**Avoid:** Changing routing, dependency injection, models, and serialization together when the error already identifies one boundary.

**Why it matters:** Methodical troubleshooting narrows uncertainty; unrelated changes destroy evidence and create additional possible causes.

### 2.2 Use runtime evidence

**Prefer:** Use focused breakpoints, inspect request and response shapes, verify configuration and registrations, and compare actual values with the expected contract.

**Avoid:** Guessing repeatedly, changing code without running it, or throwing and catching exceptions merely to discover values that a debugger can show directly.

**Why it matters:** Effective debugging is an evidence-gathering process rather than a sequence of speculative edits.

### 2.3 Change one variable at a time

**Prefer:** Keep each diagnostic change small, run the relevant path, and either retain the proven fix or revert the experiment mentally before the next hypothesis.

**Avoid:** Large speculative rewrites while the cause is unknown.

**Why it matters:** Controlled experiments preserve causality and make recovery easier when a hypothesis is wrong.

### 2.4 Use documentation precisely

**Prefer:** Consult official documentation for exact syntax, configuration, or an unfamiliar edge case while stating what is being verified.

**Avoid:** Copying the first search result, searching repeatedly for basic concepts, or using a snippet that cannot be explained.

**Why it matters:** Looking up syntax is normal; accepting code without understanding its behaviour is not.

### 2.5 Use AI as assistance, not delegation

**Prefer:** Use permitted autocomplete or lightweight prompts like accelerated documentation lookup, then read, test, and explain every accepted change.

**Avoid:** Agentic workflows during the interview, wholesale generated solutions, or accepting registrations, lifetimes, tests, and fixes without validating them.

**Why it matters:** Generated code can accelerate work but exposes shallow understanding when the engineer cannot evaluate or repair it.

## 3. Communication and Answer Quality

### 3.1 Drive the pair-programming session

**Prefer:** State the plan, invite relevant input, explain decisions at useful moments, and continue making progress without waiting for constant approval.

**Avoid:** Long silent periods, narrating every keystroke, or expecting interviewers to lead and rescue each step.

**Why it matters:** Pairing evaluates collaboration and autonomy together. Reasoning must be observable without stopping delivery.

### 3.2 Explain decisions with a compact structure

**Prefer:** Give the decision, reason, important trade-off, supporting example or evidence, and next step.

**Avoid:** Naming a pattern or library as the entire justification, or giving a long theoretical answer that never addresses the decision at hand.

**Why it matters:** Structured reasoning lets others assess intent, challenge assumptions, and distinguish a shortcut from a gap.

### 3.3 Answer the question that was asked

**Prefer:** Lead with a direct answer, then add one concrete example and only the nuance needed for the question.

**Avoid:** Broad background monologues, unrelated technology lists, or answers that force the interviewer to extract the conclusion.

**Why it matters:** Relevant answers demonstrate comprehension and make technical depth easier to evaluate.

### 3.4 Make experience examples concrete

**Prefer:** State the situation, personal responsibility, action, result, and what changed afterward.

**Avoid:** Describing only what "the team" did, naming tools without explaining their use, or omitting the outcome.

**Why it matters:** Interviewers need evidence of the candidate's contribution and judgement, not only exposure to a project or technology.

### 3.5 Surface uncertainty constructively

**Prefer:** State what is known, what is uncertain, how it will be verified, and which safe work can continue meanwhile.

**Avoid:** Bluffing, silently guessing, or repeatedly apologising without taking a diagnostic step.

**Why it matters:** Honest uncertainty paired with a verification plan builds trust and demonstrates practical problem solving.

### 3.6 Treat prompts as new evidence

**Prefer:** Pause, evaluate the suggestion, explain whether it changes the design, and adjust promptly when it reveals a better path.

**Avoid:** Defending the initial approach automatically, accepting every hint without thought, or changing direction without explaining why.

**Why it matters:** Collaborative engineering requires both receptiveness and independent judgement.

### 3.7 Connect terminology to practice

**Prefer:** Link SOLID, clean architecture, microservices, scalability, or testing terminology to a concrete dependency, failure mode, trade-off, or project example.

**Avoid:** Using vocabulary density as a substitute for implementation knowledge or lived experience.

**Why it matters:** Follow-up questions test whether concepts are understood and applied rather than memorised.

## 4. Testing and Quality Discussions

### 4.1 Describe a risk-based test strategy

**Prefer:** Start from user-critical behaviour and failure risk, then assign unit, integration, contract, end-to-end, and exploratory tests according to the confidence each must provide.

**Avoid:** Reciting a test pyramid without connecting it to the system, or proposing exhaustive end-to-end coverage for every branch.

**Why it matters:** A useful strategy spends testing effort where defects would be most costly or likely.

### 4.2 Distinguish test levels clearly

**Prefer:** Explain the boundary under test, real and substituted dependencies, execution environment, speed, and failure diagnostic value for each test level.

**Avoid:** Calling a host or real database test a unit test, or treating all automated tests as interchangeable.

**Why it matters:** Precise test classification supports balanced coverage and prevents slow, brittle suites.

### 4.3 Explain testability as a design property

**Prefer:** Discuss focused responsibilities, explicit dependencies, deterministic inputs, stable boundaries, and observable outputs.

**Avoid:** Equating testability with creating an interface for every class or choosing a mocking library.

**Why it matters:** Testability comes primarily from design and controlled dependencies, not tooling.

### 4.4 Own quality without relying on dedicated QA

**Prefer:** Describe how engineers prevent, detect, and respond to defects through tests, review, observability, staged delivery, and collaboration with QA where available.

**Avoid:** Treating quality as another team's responsibility or assuming a final manual QA phase will compensate for untestable code.

**Why it matters:** Modern product teams expect engineers to own feature quality throughout delivery and operation.

### 4.5 Discuss manual testing as a complement

**Prefer:** Use exploratory and manual checks for discovery, usability, unusual workflows, and final integrated confidence while automating repeatable regression checks.

**Avoid:** Claiming that all manual testing is waste or that repeated manual verification is a sufficient regression strategy.

**Why it matters:** Manual and automated testing answer different questions and work best as complementary controls.

### 4.6 Explain TDD as a feedback technique

**Prefer:** Describe how a failing example guides design, implementation, and refactoring, and when another workflow may be more efficient.

**Avoid:** Presenting TDD as mandatory ceremony or merely writing tests before code without a feedback loop.

**Why it matters:** The value of TDD is rapid design feedback and safe change, not the order of file creation.

## 5. Teamwork and Collaboration

### 5.1 Use pairing for more than unblocking

**Prefer:** Use pair programming for knowledge sharing, difficult design decisions, rapid feedback, onboarding, risk reduction, and collective ownership.

**Avoid:** Treating pairing only as an emergency mechanism when one engineer is stuck.

**Why it matters:** Pairing can improve both flow and team capability even when neither participant has a blocker.

### 5.2 Resolve disagreements through shared goals

**Prefer:** Clarify the user or business outcome, compare evidence and trade-offs, run a small experiment when possible, and record the resulting decision.

**Avoid:** Arguing from seniority, personal preference, or attachment to the original proposal.

**Why it matters:** Productive disagreement improves decisions without damaging trust or delivery.

### 5.3 Communicate across functions

**Prefer:** Adapt detail for engineers, product, design, operations, and stakeholders while preserving the decision, risk, and requested action.

**Avoid:** Assuming every audience shares the same technical context or hiding trade-offs behind jargon.

**Why it matters:** Cross-functional teams depend on shared understanding, not merely accurate implementation.

### 5.4 Make decisions discoverable

**Prefer:** Capture important assumptions, alternatives, decisions, ownership, and follow-up work in tickets, lightweight decision records, or relevant documentation.

**Avoid:** Relying on memory, private conversations, or documentation that records conclusions without rationale.

**Why it matters:** Discoverable decisions reduce repeated debate and help future changes account for the original constraints.

### 5.5 Give and receive feedback specifically

**Prefer:** Tie feedback to an observable behaviour, its impact, and a concrete improvement; evaluate incoming feedback before responding.

**Avoid:** Personal labels, vague praise or criticism, defensiveness, or performative agreement without a change in understanding.

**Why it matters:** Specific feedback supports improvement and keeps technical discussion separate from personal judgement.

### 5.6 Demonstrate appropriate autonomy

**Prefer:** Make reversible decisions independently, escalate consequential uncertainty early, and involve others when their context or authority changes the outcome.

**Avoid:** Waiting for detailed instructions on every step or making high-impact decisions without relevant stakeholders.

**Why it matters:** Strong contributors balance initiative with alignment rather than maximising either independence or consensus.

## 6. Ownership and Production Responsibility

### 6.1 Demonstrate end-to-end ownership

**Prefer:** Connect requirements, implementation, tests, deployment, monitoring, incident response, stakeholder communication, and follow-up work.

**Avoid:** Treating architecture, QA, deployment, monitoring, and production support as concerns that always belong to other teams.

**Why it matters:** Senior roles expect ownership of outcomes across the service lifecycle rather than completion of coding tasks alone.

### 6.2 Discuss production readiness proportionately

**Prefer:** Prioritise the production concerns most relevant to the service: failure handling, security, observability, configuration, deployment, rollback, data integrity, and support.

**Avoid:** Listing every enterprise capability without relating it to risk, scale, or domain requirements.

**Why it matters:** Production judgement is the ability to identify the next real risk, not to maximise the number of technologies mentioned.

### 6.3 Explain monitoring as an operating system

**Prefer:** Connect service objectives to metrics, traces, logs, dashboards, alerts, runbooks, ownership, and actions when a signal degrades.

**Avoid:** Stopping at "add logging" or naming monitoring products without explaining the signals and response process.

**Why it matters:** Observability creates value only when it supports detection, diagnosis, and action.

### 6.4 Explain deployment strategies concretely

**Prefer:** Describe traffic movement, compatibility, verification, rollback, and trade-offs for rolling, blue/green, canary, or feature-flagged delivery.

**Avoid:** Naming a strategy without explaining how a failed release is detected and reversed.

**Why it matters:** Deployment knowledge is practical when it shows how change reaches users safely.

### 6.5 Treat incidents as learning opportunities

**Prefer:** Stabilise impact, communicate clearly, gather evidence, restore service safely, identify contributing system factors, and track preventive actions.

**Avoid:** Searching for an individual to blame, making risky changes without coordination, or ending the process when service is restored.

**Why it matters:** Mature incident response improves both present reliability and the system's future resilience.

## 7. Scope, Technical Debt, and Incomplete Work

### 7.1 Explain technical debt with context

**Prefer:** Describe debt as a deliberate or emergent future cost caused by constraints, changing requirements, aging assumptions, or deferred work; explain how it is recorded and prioritised.

**Avoid:** Reducing technical debt to "fast code is bad code" or claiming that all shortcuts are unacceptable.

**Why it matters:** Mature teams balance delivery and maintainability by making future cost visible and manageable.

### 7.2 Handle scope change explicitly

**Prefer:** Reassess value, risk, dependencies, acceptance criteria, and remaining capacity with stakeholders, then agree what changes or is deferred.

**Avoid:** Absorbing every change silently, treating change as process failure, or promising the original scope and date without a trade-off.

**Why it matters:** Scope change is a normal product constraint that requires transparent reprioritisation.

### 7.3 Describe incomplete work as a plan

**Prefer:** State what is complete, what is incomplete, the current risk, the next highest-value step, and the realistic effort or ownership needed.

**Avoid:** A vague promise to "refactor and add tests" or a long wishlist with no priority.

**Why it matters:** A credible handoff demonstrates self-awareness, risk management, and respect for the next engineer or stakeholder.

### 7.4 Delegate with clear outcomes

**Prefer:** Match work to capability and growth goals, define the result and constraints, provide context, and establish review points without prescribing every action.

**Avoid:** Delegating only low-value tasks, transferring responsibility without support, or retaining all decisions while assigning only implementation.

**Why it matters:** Effective delegation expands team capacity and develops engineers while preserving accountability.
