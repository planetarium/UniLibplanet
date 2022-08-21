LIBPLANET_UNITY_DIR=./Libplanet.Unity
DLLS_DIR=./Libplanet.Unity/bin/Release/netstandard2.1/
RUNTIME_DLL_DIR=./Libplanet.Unity/runtimes/
PLUGINS_DIR=./UniLibplanet/Assets/Plugins/

RELATIVE_PATH_SCRIPT_DIR=$(dirname $0)
cd $RELATIVE_PATH_SCRIPT_DIR
ABSOLUTE_PATH_SCRIPT_DIR=`pwd -P`
PARENT_PATH=$(dirname $ABSOLUTE_PATH_SCRIPT_DIR)
cd $PARENT_PATH

echo "Starting DLL build..."
dotnet build $LIBPLANET_UNITY_DIR --configuration Release

if [ -d $PLUGINS_DIR ] ; then
    echo "Existing $PLUGINS_DIR found"
    echo "Removing existing $PLUGINS_DIR..."
    rm -rf $PLUGINS_DIR
fi

mkdir $PLUGINS_DIR
echo "Copying DLLs to target directory..."
find $DLLS_DIR -name "[^Unity]*.dll" -exec cp '{}' $PLUGINS_DIR \;
rm $PLUGINS_DIR/"Microsoft.CSharp.dll"
rm $PLUGINS_DIR/"System.ServiceModel.Primitives.dll"

mkdir $PLUGINS_DIR/"runtimes"
ls -d $RUNTIME_DLL_DIR* | xargs -i cp -r {} $PLUGINS_DIR/"runtimes"

echo "Removing artifacts..."
rm -rf '.\Libplanet.Unity\bin\'
rm -rf '.\Libplanet.Unity\obj\'