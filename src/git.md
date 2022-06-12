# Change commit date

To set the current date we need only to do the following:

```bash
GIT_COMMITTER_DATE=$(date) git commit --amend --no-edit --date $(date)
```

NOTE: the `GIT_COMMITTER_DATE` is for the committer while the `--date` is for the author.

it is possible to apply the _author_ date to the _committer_ date by using the `--committer-date-is-author-date` flag in the `git rebase` command. For example:

```bash
$ git rebase HEAD~x --committer-date-is-author-date
```

Here on example of how to do it manually and wrong.

```bash
GIT_COMMITTER_DATE="Tue Jun 7  4:44:34 PM -03 2022" git commit --amend --no-edit --date "Tue Jun 7  4:44:34 PM -03 2022"
```
