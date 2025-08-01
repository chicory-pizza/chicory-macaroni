// Chicory Macaroni
// https://macaroni.chicory.pizza

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Scripting;

const string MacaroniFileName = "macaroni.win";

UndertaleData data = null;

// Get the file paths
string windowsDataFile = PromptDataFile("Select data.win from the Windows edition", "win");
if (windowsDataFile == null)
{
    return;
}

string macDataFile = PromptDataFile("Select game.ios from the Mac edition", "ios");
if (macDataFile == null)
{
    return;
}

// Load the Windows edition, export the shaders
SetProgressBar("Baking Macaroni...", "Working, please wait...", 0, 5);

await LoadFile(windowsDataFile);
if (data?.GeneralInfo?.DisplayName?.Content == "Chicory_A_Colorful_Tale")
{
    throw new ScriptException("You are loading the Mac data file for the Windows edition, or this data file is already converted.");
}
else if (!IsValidWindowsDataFile(data))
{
    throw new ScriptException("The data file for the Windows edition is not the Chicory game.");
}

IncrementProgressParallel();

string tempFolder = GetTempFolder();
ExportShaderData(tempFolder);
IncrementProgressParallel();

// Now load the Mac edition, import the shaders
await LoadFile(macDataFile);
if (data?.GeneralInfo?.DisplayName?.Content == "Chicory: A Colorful Tale")
{
    throw new ScriptException("You are loading the Windows data file for the Mac edition.");
}
else if (data?.GeneralInfo?.DisplayName?.Content != "Chicory_A_Colorful_Tale")
{
    throw new ScriptException("The data file for the Mac edition is not the Chicory game.");
}

IncrementProgressParallel();

ImportShaders importer = new ImportShaders(data, tempFolder);
importer.StartImport();
IncrementProgressParallel();

// Save
await SaveFile(Path.GetDirectoryName(windowsDataFile) + "\\" + MacaroniFileName);

// Cleanup
Directory.Delete(tempFolder, true);

HideProgressBar();
ScriptMessage("Finished!\n\n" + MacaroniFileName + " is now saved to your Windows game folder.\n\nYou should be able to open that file to start editing game scripts.\n\n---\n\nTo run the game:\n\n1. Copy Runner.exe and RunMacaroni.bat to your Windows game folder\n\n2. Open RunMacaroni.bat");

// Functions
string PromptDataFile(string title, string extension)
{
    OpenFileDialog dlg = new OpenFileDialog();
    dlg.DefaultExt = extension;
    dlg.Filter = "Game Maker Studio data files (." + extension + ")|*." + extension + "|All files|*";
    dlg.Title = title;

    return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
}

async Task LoadFile(string filename)
{
    await Task.Run(() =>
    {
        data = null;

        try
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                data = UndertaleIO.Read(stream, null);
            }
        }
        catch (Exception e)
        {
            throw new ScriptException("There was a problem loading the data file:\n" + e.Message);
        }

        if (data == null)
        {
            throw new ScriptException("There was a problem loading the data file.");
        }
    });
}

async Task SaveFile(string filename)
{
    await Task.Run(() =>
    {
        try
        {
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                UndertaleIO.Write(stream, data);
            }
        }
        catch (Exception e)
        {
            throw new ScriptException("There was a problem saving the data file:\n" + e.Message);
        }
    });
}

string GetTempFolder()
{
    string tempFolder = Path.GetTempFileName();
    File.Delete(tempFolder);
    Directory.CreateDirectory(tempFolder);

    return tempFolder + "\\";
}

bool IsValidWindowsDataFile(UndertaleData data)
{
    if (data?.GeneralInfo?.DisplayName?.Content == "Chicory: A Colorful Tale") // .59
    {
        return true;
    }

    if (data?.GeneralInfo?.DisplayName?.Content == "Created with GameMaker" && data?.GeneralInfo?.Name?.Content == "paintdog") // Steam Windows .66
    {
        return true;
    }

    return false;
}

// Based from ExportAllShaders.csx
#region Export
void ExportShaderData(string exportFolder)
{
    foreach (UndertaleShader shader in data.Shaders)
    {
        if (shader is null)
        {
            continue;
        }

        string exportBase = Path.Combine(exportFolder, shader.Name.Content);
        Directory.CreateDirectory(exportBase);

        File.WriteAllText(Path.Combine(exportBase, "Type.txt"), shader.Type.ToString());
        File.WriteAllText(Path.Combine(exportBase, "GLSL_ES_Fragment.txt"), shader.GLSL_ES_Fragment.Content);
        File.WriteAllText(Path.Combine(exportBase, "GLSL_ES_Vertex.txt"), shader.GLSL_ES_Vertex.Content);
        File.WriteAllText(Path.Combine(exportBase, "GLSL_Fragment.txt"), shader.GLSL_Fragment.Content);
        File.WriteAllText(Path.Combine(exportBase, "GLSL_Vertex.txt"), shader.GLSL_Vertex.Content);
        File.WriteAllText(Path.Combine(exportBase, "HLSL9_Fragment.txt"), shader.HLSL9_Fragment.Content);
        File.WriteAllText(Path.Combine(exportBase, "HLSL9_Vertex.txt"), shader.HLSL9_Vertex.Content);
        if (!shader.HLSL11_VertexData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "HLSL11_VertexData.bin"), shader.HLSL11_VertexData.Data);
        if (!shader.HLSL11_PixelData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "HLSL11_PixelData.bin"), shader.HLSL11_PixelData.Data);
        if (!shader.PSSL_VertexData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "PSSL_VertexData.bin"), shader.PSSL_VertexData.Data);
        if (!shader.PSSL_PixelData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "PSSL_PixelData.bin"), shader.PSSL_PixelData.Data);
        if (!shader.Cg_PSVita_VertexData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "Cg_PSVita_VertexData.bin"), shader.Cg_PSVita_VertexData.Data);
        if (!shader.Cg_PSVita_PixelData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "Cg_PSVita_PixelData.bin"), shader.Cg_PSVita_PixelData.Data);
        if (!shader.Cg_PS3_VertexData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "Cg_PS3_VertexData.bin"), shader.Cg_PS3_VertexData.Data);
        if (!shader.Cg_PS3_PixelData.IsNull)
            File.WriteAllBytes(Path.Combine(exportBase, "Cg_PS3_PixelData.bin"), shader.Cg_PS3_PixelData.Data);

        StringBuilder vertexSb = new();
        for (var i = 0; i < shader.VertexShaderAttributes.Count; i++)
        {
            vertexSb.AppendLine(shader.VertexShaderAttributes[i].Name.Content);
        }
        File.WriteAllText(Path.Combine(exportBase, "VertexShaderAttributes.txt"), vertexSb.ToString());
    }
}
#endregion

// Based from ImportShaders.csx
#region Import
public class ImportShaders(UndertaleData data, string importFolder)
{
    private UndertaleData Data = data;
    private string ImportFolder = importFolder;

    public void StartImport()
    {
        var shadersToModify = Directory.GetDirectories(ImportFolder).Select(x => Path.GetFileName(x));

        List<string> currentList = new List<string>();
        string res = "";

        foreach (string shaderName in shadersToModify)
        {
            currentList.Clear();
            for (int j = 0; j < Data.Shaders.Count; j++)
            {
                string x = Data.Shaders[j].Name.Content;
                res += (x + "\n");
                currentList.Add(x);
            }
            if (Data.Shaders.ByName(shaderName) != null)
            {
                Data.Shaders.Remove(Data.Shaders.ByName(shaderName));
                AddShader(shaderName);
                Reorganize<UndertaleShader>(Data.Shaders, currentList);
            }
            else
                AddShader(shaderName);
        }
    }

    private void AddShader(string shader_name)
    {
        UndertaleShader new_shader = new UndertaleShader();
        new_shader.Name = Data.Strings.MakeString(shader_name);
        string localImportDir = ImportFolder + "/" + shader_name + "/";
        if (File.Exists(localImportDir + "Type.txt"))
        {
            string shader_type = File.ReadAllText(localImportDir + "Type.txt");
            if (shader_type.Contains("GLSL_ES"))
                new_shader.Type = UndertaleShader.ShaderType.GLSL_ES;
            else if (shader_type.Contains("GLSL"))
                new_shader.Type = UndertaleShader.ShaderType.GLSL;
            else if (shader_type.Contains("HLSL9"))
                new_shader.Type = UndertaleShader.ShaderType.HLSL9;
            else if (shader_type.Contains("HLSL11"))
                new_shader.Type = UndertaleShader.ShaderType.HLSL11;
            else if (shader_type.Contains("PSSL"))
                new_shader.Type = UndertaleShader.ShaderType.PSSL;
            else if (shader_type.Contains("Cg_PSVita"))
                new_shader.Type = UndertaleShader.ShaderType.Cg_PSVita;
            else if (shader_type.Contains("Cg_PS3"))
                new_shader.Type = UndertaleShader.ShaderType.Cg_PS3;
            else
                new_shader.Type = UndertaleShader.ShaderType.GLSL_ES;
        }
        else
            new_shader.Type = UndertaleShader.ShaderType.GLSL_ES;
        if (File.Exists(localImportDir + "GLSL_ES_Fragment.txt"))
            new_shader.GLSL_ES_Fragment = Data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_ES_Fragment.txt"));
        else
            new_shader.GLSL_ES_Fragment = Data.Strings.MakeString("");
        if (File.Exists(localImportDir + "GLSL_ES_Vertex.txt"))
            new_shader.GLSL_ES_Vertex = Data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_ES_Vertex.txt"));
        else
            new_shader.GLSL_ES_Vertex = Data.Strings.MakeString("");
        if (File.Exists(localImportDir + "GLSL_Fragment.txt"))
            new_shader.GLSL_Fragment = Data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_Fragment.txt"));
        else
            new_shader.GLSL_Fragment = Data.Strings.MakeString("");
        if (File.Exists(localImportDir + "GLSL_Vertex.txt"))
            new_shader.GLSL_Vertex = Data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_Vertex.txt"));
        else
            new_shader.GLSL_Vertex = Data.Strings.MakeString("");
        if (File.Exists(localImportDir + "HLSL9_Fragment.txt"))
            new_shader.HLSL9_Fragment = Data.Strings.MakeString(File.ReadAllText(localImportDir + "HLSL9_Fragment.txt"));
        else
            new_shader.HLSL9_Fragment = Data.Strings.MakeString("");
        if (File.Exists(localImportDir + "HLSL9_Vertex.txt"))
            new_shader.HLSL9_Vertex = Data.Strings.MakeString(File.ReadAllText(localImportDir + "HLSL9_Vertex.txt"));
        else
            new_shader.HLSL9_Vertex = Data.Strings.MakeString("");
        if (File.Exists(localImportDir + "HLSL11_VertexData.bin"))
        {
            new_shader.HLSL11_VertexData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.HLSL11_VertexData.Data = File.ReadAllBytes(localImportDir + "HLSL11_VertexData.bin");
            new_shader.HLSL11_VertexData.IsNull = false;
        }
        if (File.Exists(localImportDir + "HLSL11_PixelData.bin"))
        {
            new_shader.HLSL11_PixelData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.HLSL11_PixelData.IsNull = false;
            new_shader.HLSL11_PixelData.Data = File.ReadAllBytes(localImportDir + "HLSL11_PixelData.bin");
        }
        if (File.Exists(localImportDir + "PSSL_VertexData.bin"))
        {
            new_shader.PSSL_VertexData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.PSSL_VertexData.IsNull = false;
            new_shader.PSSL_VertexData.Data = File.ReadAllBytes(localImportDir + "PSSL_VertexData.bin");
        }
        if (File.Exists(localImportDir + "PSSL_PixelData.bin"))
        {
            new_shader.PSSL_PixelData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.PSSL_PixelData.IsNull = false;
            new_shader.PSSL_PixelData.Data = File.ReadAllBytes(localImportDir + "PSSL_PixelData.bin");
        }
        if (File.Exists(localImportDir + "Cg_PSVita_VertexData.bin"))
        {
            new_shader.Cg_PSVita_VertexData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.Cg_PSVita_VertexData.IsNull = false;
            new_shader.Cg_PSVita_VertexData.Data = File.ReadAllBytes(localImportDir + "Cg_PSVita_VertexData.bin");
        }
        if (File.Exists(localImportDir + "Cg_PSVita_PixelData.bin"))
        {
            new_shader.Cg_PSVita_PixelData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.Cg_PSVita_PixelData.IsNull = false;
            new_shader.Cg_PSVita_PixelData.Data = File.ReadAllBytes(localImportDir + "Cg_PSVita_PixelData.bin");
        }
        if (File.Exists(localImportDir + "Cg_PS3_VertexData.bin"))
        {
            new_shader.Cg_PS3_VertexData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.Cg_PS3_VertexData.IsNull = false;
            new_shader.Cg_PS3_VertexData.Data = File.ReadAllBytes(localImportDir + "Cg_PS3_VertexData.bin");
        }
        if (File.Exists(localImportDir + "Cg_PS3_PixelData.bin"))
        {
            new_shader.Cg_PS3_PixelData = new UndertaleShader.UndertaleRawShaderData();
            new_shader.Cg_PS3_PixelData.IsNull = false;
            new_shader.Cg_PS3_PixelData.Data = File.ReadAllBytes(localImportDir + "Cg_PS3_PixelData.bin");
        }
        if (File.Exists(localImportDir + "VertexShaderAttributes.txt"))
        {
            string line;
            // Read the file and display it line by line.
            StreamReader file = new StreamReader(localImportDir + "VertexShaderAttributes.txt");
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (line != "")
                {
                    UndertaleShader.VertexShaderAttribute vertex_x = new UndertaleShader.VertexShaderAttribute();
                    vertex_x.Name = Data.Strings.MakeString(line);
                    new_shader.VertexShaderAttributes.Add(vertex_x);
                }
            }
            file.Close();
        }
        Data.Shaders.Add(new_shader);
    }

    private static void Reorganize<T>(IList<T> list, List<string> order) where T : UndertaleNamedResource, new()
    {
        Dictionary<string, T> temp = new Dictionary<string, T>();
        for (int i = 0; i < list.Count; i++)
        {
            T asset = list[i];
            string assetName = asset.Name?.Content;
            if (order.Contains(assetName))
            {
                temp[assetName] = asset;
            }
        }

        List<T> addOrder = new List<T>();
        for (int i = order.Count - 1; i >= 0; i--)
        {
            T asset;
            try
            {
                asset = temp[order[i]];
            }
            catch (Exception e)
            {
                throw new ScriptException("Missing asset with name \"" + order[i] + "\"");
            }
            addOrder.Add(asset);
        }

        foreach (T asset in addOrder)
            list.Remove(asset);
        foreach (T asset in addOrder)
            list.Insert(0, asset);
    }
}
#endregion
