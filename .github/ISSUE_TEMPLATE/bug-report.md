---
name: BUG REPORT
about: Create an error ticket to help us where there are still shortcomings.
title: "[BUG]"
labels: BUG
assignees: ''

---

# 🐛 Guide: How to Create a Bug Ticket

## 1. Describe the Story

Provide context around the bug:

- **What were you doing** before the bug occurred?
- **How did the bug happen?** (e.g. after an update, during a specific action, randomly)
- **Is the bug reproducible?** If yes, how?

---

## 2. Process for Identifying the Problem

Write down step by step how the bug occurs and what you observed.

### 🎨 Graphics Bug

If it is a graphical issue, try to categorize it into one of the following:

| # | Symptom | Likely Cause |
|---|---------|--------------|
| 1 | Individual blocks or objects look wrong | **Textures** |
| 2 | Entire graphic effects are not working | **Shaders** |
| 3 | Chunks have visual problems | **Meshes** |
| 4 | The game is generally broken due to graphics | **Render API / Graphics API / Logic Error** |

### ⚙️ Physics Bug

If the game physics are affected, describe **explicitly**:

- Which object or interaction is affected?
- What exactly is behaving incorrectly (e.g. collision, movement, gravity)?
- Under what circumstances does the bug occur?

### ❓ Unknown Source

If you are unsure which category the bug belongs to:

1. Try to isolate the bug **step by step**.
2. Describe the behavior as **precisely and clearly** as possible.
3. Use an **LLM** (e.g. ChatGPT, Claude) as a support tool to help with categorization.

---

## 3. Screenshots

> 📸 **Please always attach screenshots!**
> They help us understand and reproduce the issue much faster and more precisely.

---

## 4. Required Information

Please include the following details in **every ticket**:

- **Game Version:** `vX.X.X`
- **Operating System (OS):**
- **Graphics Card (GPU):**
- **Processor (CPU):**
- **RAM:**
