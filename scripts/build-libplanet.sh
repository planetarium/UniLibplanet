LIBPLANET_UNITY_DIR=./Libplanet.Unity
DLLS_DIR=./Libplanet.Unity/bin/Release/netstandard2.1/
RUNTIME_DLL_DIR=./Libplanet.Unity/runtimes/
PLUGINS_DIR=./UniLibplanet/Assets/Plugins/

#EXCLUDES=("Microsoft.CSharp.dll", "System.ServiceModel.Primitives.dll", "Unity*.dll")
#ARTIFACT_DIRS=@(".\Libplanet.Unity\bin\", ".\Libplanet.Unity\obj\")

echo "Starting DLL build..."
dotnet build $LIBPLANET_UNITY_DIR --configuration Release

if [ -d $PLUGINS_DIR ] ; then
    echo "Existing $PLUGINS_DIR found"
    echo "Removing existing $PLUGINS_DIR..."
    rm -rf $PLUGINS_DIR
fi

#EXCLUDES=("Microsoft.CSharp.dll", "System.ServiceModel.Primitives.dll", "Unity*.dll")
mkdir $PLUGINS_DIR
echo "Copying DLLs to target directory..."
find $DLLS_DIR -name "[^Unity]*.dll" -exec cp '{}' $PLUGINS_DIR \;
rm $PLUGINS_DIR/"Microsoft.CSharp.dll"
rm $PLUGINS_DIR/"System.ServiceModel.Primitives.dll"

mkdir $PLUGINS_DIR/"runtimes"
ls -d $RUNTIME_DLL_DIR* | xargs -i cp -r {} $PLUGINS_DIR/"runtimes"
ls -d $RUNTIME_DLL_DIR* | xargs -i echo {}

#ARTIFACT_DIRS=@(".\Libplanet.Unity\bin\", ".\Libplanet.Unity\obj\")
echo "Removing artifacts..."
rm -rf '.\Libplanet.Unity\bin\'
rm -rf '.\Libplanet.Unity\obj\'