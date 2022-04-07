// Chicory Macaroni
// https://github.com/chicory-pizza/chicory-macaroni

using System;
using System.Collections.Generic;
using System.IO;
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
else if (data?.GeneralInfo?.DisplayName?.Content != "Chicory: A Colorful Tale")
{
    throw new ScriptException("The data file for the Windows edition is not the Chicory game.");
}

IncProgressP();

string tempFolder = GetTempFolder();
ExportShaderData(tempFolder);
IncProgressP();

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

IncProgressP();

ImportShaderData.Import(data, tempFolder);
IncProgressP();

// Save
await SaveFile(Path.GetDirectoryName(windowsDataFile) + "\\" + MacaroniFileName);

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

// Based from ExportShaderData.csx
#region Export
void ExportShaderData(string exportFolder)
{
    foreach (UndertaleShader shader in data.Shaders)
    {
        string exportBase = (exportFolder + "/" + shader.Name.Content + "/");
        Directory.CreateDirectory(exportBase);

        File.WriteAllText(exportBase + "Type.txt", shader.Type.ToString());
        File.WriteAllText(exportBase + "GLSL_ES_Fragment.txt", shader.GLSL_ES_Fragment.Content);
        File.WriteAllText(exportBase + "GLSL_ES_Vertex.txt", shader.GLSL_ES_Vertex.Content);
        File.WriteAllText(exportBase + "GLSL_Fragment.txt", shader.GLSL_Fragment.Content);
        File.WriteAllText(exportBase + "GLSL_Vertex.txt", shader.GLSL_Vertex.Content);
        File.WriteAllText(exportBase + "HLSL9_Fragment.txt", shader.HLSL9_Fragment.Content);
        File.WriteAllText(exportBase + "HLSL9_Vertex.txt", shader.HLSL9_Vertex.Content);
        if (shader.HLSL11_VertexData.IsNull == false)
            File.WriteAllBytes(exportBase + "HLSL11_VertexData.bin", shader.HLSL11_VertexData.Data);
        if (shader.HLSL11_PixelData.IsNull == false)
            File.WriteAllBytes(exportBase + "HLSL11_PixelData.bin", shader.HLSL11_PixelData.Data);
        if (shader.PSSL_VertexData.IsNull == false)
            File.WriteAllBytes(exportBase + "PSSL_VertexData.bin", shader.PSSL_VertexData.Data);
        if (shader.PSSL_PixelData.IsNull == false)
            File.WriteAllBytes(exportBase + "PSSL_PixelData.bin", shader.PSSL_PixelData.Data);
        if (shader.Cg_PSVita_VertexData.IsNull == false)
            File.WriteAllBytes(exportBase + "Cg_PSVita_VertexData.bin", shader.Cg_PSVita_VertexData.Data);
        if (shader.Cg_PSVita_PixelData.IsNull == false)
            File.WriteAllBytes(exportBase + "Cg_PSVita_PixelData.bin", shader.Cg_PSVita_PixelData.Data);
        if (shader.Cg_PS3_VertexData.IsNull == false)
            File.WriteAllBytes(exportBase + "Cg_PS3_VertexData.bin", shader.Cg_PS3_VertexData.Data);
        if (shader.Cg_PS3_PixelData.IsNull == false)
            File.WriteAllBytes(exportBase + "Cg_PS3_PixelData.bin", shader.Cg_PS3_PixelData.Data);
        string vertex = null;
        for (var i = 0; i < shader.VertexShaderAttributes.Count; i++)
        {
            if (vertex == null)
                vertex = "";
            vertex += shader.VertexShaderAttributes[i].Name.Content;
            vertex += "\n";
        }
        File.WriteAllText(exportBase + "VertexShaderAttributes.txt", ((vertex != null) ? vertex : ""));
    }
}
#endregion

// Based from ImportShaderData.csx
#region Import
public class ImportShaderData
{
    public static void Import(UndertaleData data, string importFolder)
    {
        string[] dirFiles = Directory.GetFiles(importFolder, "*.*", SearchOption.AllDirectories);
        List<string> shadersToModify = new List<string>();

        foreach (string file in dirFiles)
        {
            shadersToModify.Add(Path.GetDirectoryName(file).Replace(importFolder, ""));
        }

        List<string> currentList = new List<string>();
        string res = "";

        for (var i = 0; i < shadersToModify.Count; i++)
        {
            currentList.Clear();
            for (int j = 0; j < data.Shaders.Count; j++)
            {
                string x = data.Shaders[j].Name.Content;
                res += (x + "\n");
                currentList.Add(x);
            }
            if (data.Shaders.ByName(shadersToModify[i]) != null)
            {
                data.Shaders.Remove(data.Shaders.ByName(shadersToModify[i]));
                AddShader(data, importFolder, shadersToModify[i]);
                Reorganize<UndertaleShader>(data.Shaders, currentList);
            }
            else
                AddShader(data, importFolder, shadersToModify[i]);
        }
    }

    private static void AddShader(UndertaleData data, string importFolder, string shader_name)
    {
        UndertaleShader new_shader = new UndertaleShader();
        new_shader.Name = data.Strings.MakeString(shader_name);
        string localImportDir = importFolder + "/" + shader_name + "/";
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
            new_shader.GLSL_ES_Fragment = data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_ES_Fragment.txt"));
        else
            new_shader.GLSL_ES_Fragment = data.Strings.MakeString("");
        if (File.Exists(localImportDir + "GLSL_ES_Vertex.txt"))
            new_shader.GLSL_ES_Vertex = data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_ES_Vertex.txt"));
        else
            new_shader.GLSL_ES_Vertex = data.Strings.MakeString("");
        if (File.Exists(localImportDir + "GLSL_Fragment.txt"))
            new_shader.GLSL_Fragment = data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_Fragment.txt"));
        else
            new_shader.GLSL_Fragment = data.Strings.MakeString("");
        if (File.Exists(localImportDir + "GLSL_Vertex.txt"))
            new_shader.GLSL_Vertex = data.Strings.MakeString(File.ReadAllText(localImportDir + "GLSL_Vertex.txt"));
        else
            new_shader.GLSL_Vertex = data.Strings.MakeString("");
        if (File.Exists(localImportDir + "HLSL9_Fragment.txt"))
            new_shader.HLSL9_Fragment = data.Strings.MakeString(File.ReadAllText(localImportDir + "HLSL9_Fragment.txt"));
        else
            new_shader.HLSL9_Fragment = data.Strings.MakeString("");
        if (File.Exists(localImportDir + "HLSL9_Vertex.txt"))
            new_shader.HLSL9_Vertex = data.Strings.MakeString(File.ReadAllText(localImportDir + "HLSL9_Vertex.txt"));
        else
            new_shader.HLSL9_Vertex = data.Strings.MakeString("");
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
                if (line != "")
                {
                    UndertaleShader.VertexShaderAttribute vertex_x = new UndertaleShader.VertexShaderAttribute();
                    vertex_x.Name = data.Strings.MakeString(line);
                    new_shader.VertexShaderAttributes.Add(vertex_x);
                }
            }
            file.Close();
        }
        data.Shaders.Add(new_shader);
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
