FROM ubuntu:16.04

LABEL "com.github.actions.name"="Build Unity Release"
LABEL "com.github.actions.description"="Builds Unity release from project at target input directory. Currently only works with Win64 releases."
LABEL "com.github.actions.icon"="package"
LABEL "com.github.actions.color"="blue"

LABEL "repository"="https://github.com/AffectiveCognitiveInstitute/kobelu-gui"
LABEL "maintainer"="Moritz Umfahrer <moritz.umfahrer@hs-ofenburg.de>"

ENV TZ=Europe/Berlin

ADD entrypoint.sh /entrypoint.sh
RUN chmod +x entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
