DOTENV_PATH=./.env.xml

UNITY_PATHS=($(find /Applications/Unity/hub -name "Unity.app"))

if [ ${#UNITY_PATHS[@]} -eq '0' ] ; then 
    echo "No Unity Editor was found"
    exit 0
fi

echo "\nUnity Editor versions found"
PS3='Choose a version : '
select version in ${UNITY_PATHS[@]} quit; do
    if [[ " ${UNITY_PATHS[*]} " =~ " ${version} " ]]; then
        # whatever you want to do when array contains value
        echo "$version selected"
        break
    fi
    case $version in 
        quit)
            exit 0
            ;;
        *)
            echo "Invalid option $REPLY"
            ;;
    esac
done

unity_dir=$(dirname "$version")
unity_engine_dir="$version/Unity.app/Contents/Managed/UnityEngine/"
dotenv_content="
<Project>
	<PropertyGroup>
		<UNITY_DIR>$unity_dir/</UNITY_DIR>
		<UNITY_ENGINE_DIR>$unity_engine_dir</UNITY_ENGINE_DIR>
	</PropertyGroup>
</Project>
"

PS3='Overwrite it?'
if [ -f "$DOTENV_PATH" ] ; then
    echo "Existing $DOTENV_PATH found"
    select opt in yes no; do
    case $opt in 
        yes)
            echo "Removing existing $DOTENV_PATH..."
            rm "$DOTENV_PATH"
            break
            ;;
        no)
            exit 0
            ;;
        *)
            echo "Invalid option $REPLY"
            ;;
    esac
done
fi

echo "Writing new $DOTENV_PATH..."
echo "$dotenv_content" > "$DOTENV_PATH"
