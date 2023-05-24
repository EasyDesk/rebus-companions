function replace {
  find . \
	-type d -path "*/.git" -prune -o \
    -type d -path "*/bin" -prune -o \
    -type d -path "*/obj" -prune -o \
    -not -wholename "$0" \
    -not -wholename . \
    -print | sort -r | while read FILE_NAME
  do
    if [[ -f "$FILE_NAME" ]] ; then
      PATTERN=s/"$1"/"$2"/g
      NEW_FILE_NAME=`echo "$FILE_NAME" | sed -e "$PATTERN"`
      mkdir -p `dirname "$NEW_FILE_NAME"`
      mv "$FILE_NAME" "$NEW_FILE_NAME"
      sed -i "$PATTERN" "$NEW_FILE_NAME"
    else
      rmdir "$FILE_NAME" &> /dev/null || true
    fi
  done
}

replace RebusCompanion RebusCompanions
