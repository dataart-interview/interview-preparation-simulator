# Welcome to the Interview Practice Emulator 👋

> **TL;DR**
>
> 1. Put your completed Cinema Seat Service solution in [`candidate-solution/`](./candidate-solution/).
> 2. Open this repository in an AI assistant that supports repository skills.
> 3. Say: `Review my solution.`
> 4. When the review is ready, say: `Start interview practice.`
>
> The skills are optional—you can also read both guides and explore the reference solution yourself.

This tool helps DataArt .NET candidates review their coding exercise and prepare for the client interview. It looks at what your solution demonstrates, highlights useful areas to focus on, and runs an interview practice session tailored to you.

Think of it as a friendly preparation coach—not a hiring judge. 🙂

## 1. Add your solution

Put your completed exercise in [`candidate-solution/`](./candidate-solution/). If you have interviewer feedback, keep it nearby and mention it when you ask for a review.

## 2. Get preparation suggestions

Say:

```text
Review my solution.
```

The review skill reads your solution and creates one file:

```text
reports/preparation-notes.md
```

When possible, it builds and tests the solution in a disposable copy.

It contains practical technical suggestions, interview-approach suggestions, and a short list of next steps. There are no scores, tiers, pass/fail decisions, or hiring predictions.

## 3. Practise one question at a time

Say:

```text
Start interview practice.
```

Each prompt is labelled **Technical** or **Interview approach**. After every answer, the companion immediately explains what was useful, what could be stronger, and gives a short example answer before asking the next question.

You can practise without reviewing a solution first. When preparation notes or interviewer feedback are available, the questions naturally spend more time on useful areas—there are no separate modes to configure.

## What guides the companion?

- [`.NET best practices`](./docs/dotnet-best-practices.md) provide the technical topics.
- [`General engineering best practices`](./docs/general-engineering-best-practices.md) provide interview-approach topics such as communication, delivery, debugging, teamwork, and production thinking.
- [`reference-solution/`](./reference-solution/) shows one complete implementation. It is an example, not an answer key.

The reference solution is deliberately opinionated and more complete than a typical interview submission. Candidates are not expected to reproduce every feature, project boundary, library, or production concern it contains.

The skills are optional. You can simply read both guides, explore the reference solution, and use whichever ideas fit your own constraints.

The review never changes your candidate solution. Generated preparation notes stay in the gitignored `reports/` directory.
