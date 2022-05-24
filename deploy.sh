#!/usr/bin/env sh

set -e

cd Build

git init
git add -A
git commit -m 'New Deployment'
git push -f git@github.com:LariWa/BoidSimulation.git master:gh-pages

cd -