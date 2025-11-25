# Pre-Commit Quick Start Guide

## What's Fixed

✅ Removed invalid `csharp` type tag
✅ Using standard pre-commit hooks only
✅ YAML syntax now valid
✅ Compatible with latest pre-commit framework

## Installation

### Option 1: Automated Setup (Recommended)

```bash
cd /Users/austintalbot/src/scratch/mindbodyDictionary
bash setup-precommit.sh
```

### Option 2: Manual Setup

```bash
# Install pre-commit framework
pip install pre-commit

# Install git hooks
pre-commit install
pre-commit install --hook-type pre-push
```

## Verify Installation

```bash
# Check if hooks are installed
ls -la .git/hooks/pre-commit

# Test hooks
pre-commit run --all-files
```

## Usage

### Automatic (on every commit)

```bash
git add .
git commit -m "Your message"
```

Hooks run automatically. If they fail:
- Review error messages
- Fix issues (many auto-fix)
- Re-stage and commit

### Manual Execution

```bash
# Check all files
pre-commit run --all-files

# Check specific hook
pre-commit run check-json --all-files

# Check only staged files
pre-commit run
```

## Configured Hooks

| Hook | Purpose | Auto-Fix |
|------|---------|----------|
| editorconfig-checker | Enforce EditorConfig rules | ❌ |
| check-json | Validate JSON files | ❌ |
| check-yaml | Validate YAML files | ❌ |
| check-xml | Validate XML files | ❌ |
| end-of-file-fixer | Ensure files end with newline | ✅ |
| trailing-whitespace | Remove trailing spaces | ✅ |
| check-merge-conflict | Detect merge markers | ❌ |
| check-case-conflict | Detect case conflicts | ❌ |
| check-added-large-files | Prevent large files (>5MB) | ❌ |
| shellcheck | Lint shell scripts | ❌ |

## Skip Hooks

```bash
# Skip specific hook
SKIP=check-json git commit -m "Your message"

# Skip all hooks (not recommended)
git commit -m "Your message" --no-verify
```

## Troubleshooting

### Issue: "pre-commit: command not found"

```bash
pip install pre-commit
```

### Issue: "Hooks failed with exit code 1"

```bash
# Review error message
# Most hooks auto-fix - re-stage and retry
git add .
git commit -m "Your message"
```

### Issue: "Check yaml failed"

Ensure YAML files have valid syntax:
```bash
# Check specific file
pre-commit run check-yaml -- file.yaml
```

### Issue: "Merge conflicts in .pre-commit-config.yaml"

```bash
# Resolve conflicts manually
git add .pre-commit-config.yaml
pre-commit run --all-files
git commit -m "Resolve conflicts"
```

## Documentation

- **Full Setup Guide**: See `PRE_COMMIT_SETUP.md`
- **Configuration**: See `.pre-commit-config.yaml`
- **Editor Config**: See `.editorconfig`

## Team Setup

When cloning the repo, new team members should run:

```bash
bash setup-precommit.sh
```

That's it! They're ready to commit.

## Updates

Monthly maintenance:

```bash
# Update all hooks to latest versions
pre-commit autoupdate

# Run on all files
pre-commit run --all-files

# Commit changes
git add .pre-commit-config.yaml
git commit -m "chore: update pre-commit hooks"
```

## Resources

- Pre-commit Framework: https://pre-commit.com
- EditorConfig: https://editorconfig.org
- Pre-commit Hooks: https://github.com/pre-commit/pre-commit-hooks

