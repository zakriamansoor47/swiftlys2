/************************************************************************************************
 * SwiftlyS2 is a scripting framework for Source2-based games.
 * Copyright (C) 2025 Swiftly Solution SRL via Sava Andrei-Sebastian and it's contributors
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 ************************************************************************************************/

#include <api/interfaces/manager.h>
#include <scripting/scripting.h>

#include <optional>
#include <variant>

void Bridge_Convars_QueryClientConvar(int playerid, const char* cvarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->QueryClientConvar(playerid, cvarName);
}

int Bridge_Convars_AddQueryClientCvarCallback(void* callback)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return convarmanager->AddQueryClientCvarCallback([callback](int playerid, std::string cvarName, std::string cvarValue) -> void {
        ((void(*)(int, const char*, const char*))callback)(playerid, cvarName.c_str(), cvarValue.c_str());
    });
}

void Bridge_Convars_RemoveQueryClientCvarCallback(int callbackId)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->RemoveQueryClientCvarCallback(callbackId);
}

void Bridge_Convars_CreateConvarInt16(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, int16_t value, int16_t* minValue, int16_t* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarUInt16(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, uint16_t value, uint16_t* minValue, uint16_t* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarInt32(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, int32_t value, int32_t* minValue, int32_t* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarUInt32(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, uint32_t value, uint32_t* minValue, uint32_t* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarInt64(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, int64_t value, int64_t* minValue, int64_t* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarUInt64(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, uint64_t value, uint64_t* minValue, uint64_t* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarBool(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, bool value, bool* minValue, bool* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value);
}

void Bridge_Convars_CreateConvarFloat(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, float value, float* minValue, float* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarDouble(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, double value, double* minValue, double* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    std::optional<ConvarValue> minValueOptional;
    std::optional<ConvarValue> maxValueOptional;
    if (minValue != nullptr) minValueOptional = *minValue;
    if (maxValue != nullptr) maxValueOptional = *maxValue;
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value, minValueOptional, maxValueOptional);
}

void Bridge_Convars_CreateConvarColor(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, Color value, Color* minValue, Color* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value);
}

void Bridge_Convars_CreateConvarVector2D(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, Vector2D value, Vector2D* minValue, Vector2D* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value);
}

void Bridge_Convars_CreateConvarVector(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, Vector value, Vector* minValue, Vector* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value);
}

void Bridge_Convars_CreateConvarVector4D(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, Vector4D value, Vector4D* minValue, Vector4D* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value);
}

void Bridge_Convars_CreateConvarQAngle(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, QAngle value, QAngle* minValue, QAngle* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, value);
}

void Bridge_Convars_CreateConvarString(const char* convarName, int cvarType, uint64_t cvarFlags, const char* helpMessage, const char* value, const char* minValue, const char* maxValue)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->CreateConvar(convarName, (EConVarType)cvarType, cvarFlags, helpMessage, std::string(value));
}

void Bridge_Convars_DeleteConvar(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->DeleteConvar(convarName);
}

bool Bridge_Convars_ExistsConvar(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return convarmanager->ExistsConvar(convarName);
}

int Bridge_Convars_GetConvarType(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return (int)(convarmanager->GetConvarType(convarName));
}

void* Bridge_Convars_GetConvarDataAddress(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return convarmanager->GetConvarDataAddress(convarName);
}

int16_t Bridge_Convars_GetConvarValueInt16(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<int16_t>(convarmanager->GetConvarValue(convarName));
}

uint16_t Bridge_Convars_GetConvarValueUInt16(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<uint16_t>(convarmanager->GetConvarValue(convarName));
}

int32_t Bridge_Convars_GetConvarValueInt32(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<int32_t>(convarmanager->GetConvarValue(convarName));
}

uint32_t Bridge_Convars_GetConvarValueUInt32(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<uint32_t>(convarmanager->GetConvarValue(convarName));
}

int64_t Bridge_Convars_GetConvarValueInt64(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<int64_t>(convarmanager->GetConvarValue(convarName));
}

uint64_t Bridge_Convars_GetConvarValueUInt64(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<uint64_t>(convarmanager->GetConvarValue(convarName));
}

bool Bridge_Convars_GetConvarValueBool(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<bool>(convarmanager->GetConvarValue(convarName));
}

float Bridge_Convars_GetConvarValueFloat(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<float>(convarmanager->GetConvarValue(convarName));
}

double Bridge_Convars_GetConvarValueDouble(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<double>(convarmanager->GetConvarValue(convarName));
}

Color Bridge_Convars_GetConvarValueColor(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<Color>(convarmanager->GetConvarValue(convarName));
}

Vector2D Bridge_Convars_GetConvarValueVector2D(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<Vector2D>(convarmanager->GetConvarValue(convarName));
}

Vector Bridge_Convars_GetConvarValueVector(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<Vector>(convarmanager->GetConvarValue(convarName));
}

Vector4D Bridge_Convars_GetConvarValueVector4D(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<Vector4D>(convarmanager->GetConvarValue(convarName));
}

QAngle Bridge_Convars_GetConvarValueQAngle(const char* convarName)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return std::get<QAngle>(convarmanager->GetConvarValue(convarName));
}

int Bridge_Convars_GetConvarValueString(char* out, const char* convarName, const char* value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    static std::string s;
    s = std::get<std::string>(convarmanager->GetConvarValue(convarName));

    if (out != nullptr) strcpy(out, s.c_str());

    return s.size();
}

void Bridge_Convars_SetConvarValueInt16(const char* convarName, int16_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueUInt16(const char* convarName, uint16_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueInt32(const char* convarName, int32_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueUInt32(const char* convarName, uint32_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueInt64(const char* convarName, int64_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueUInt64(const char* convarName, uint64_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueBool(const char* convarName, bool value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueFloat(const char* convarName, float value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueDouble(const char* convarName, double value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueColor(const char* convarName, Color value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueVector2D(const char* convarName, Vector2D value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueVector(const char* convarName, Vector value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueVector4D(const char* convarName, Vector4D value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueQAngle(const char* convarName, QAngle value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, value);
}

void Bridge_Convars_SetConvarValueString(const char* convarName, const char* value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetConvarValue(convarName, std::string(value));
}

void Bridge_Convars_SetClientConvarValueInt16(int playerid, const char* convarName, int16_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueUInt16(int playerid, const char* convarName, uint16_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueInt32(int playerid, const char* convarName, int32_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueUInt32(int playerid, const char* convarName, uint32_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueInt64(int playerid, const char* convarName, int64_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueUInt64(int playerid, const char* convarName, uint64_t value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueBool(int playerid, const char* convarName, bool value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueFloat(int playerid, const char* convarName, float value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueDouble(int playerid, const char* convarName, double value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueColor(int playerid, const char* convarName, Color value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueVector2D(int playerid, const char* convarName, Vector2D value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueVector(int playerid, const char* convarName, Vector value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueVector4D(int playerid, const char* convarName, Vector4D value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueQAngle(int playerid, const char* convarName, QAngle value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, value);
}

void Bridge_Convars_SetClientConvarValueString(int playerid, const char* convarName, const char* value)
{
    static auto convarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    convarmanager->SetClientConvar(playerid, convarName, std::string(value));
}

void Bridge_Convars_AddFlags(const char* cvarName, uint64_t flags)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    cvarmanager->AddFlags(cvarName, flags);
}

void Bridge_Convars_RemoveFlags(const char* cvarName, uint64_t flags)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    cvarmanager->RemoveFlags(cvarName, flags);
}

void Bridge_Convars_ClearFlags(const char* cvarName)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    cvarmanager->ClearFlags(cvarName);
}

uint64_t Bridge_Convars_GetFlags(const char* cvarName)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return cvarmanager->GetFlags(cvarName);
}

uint64_t Bridge_Convars_AddGlobalChangeListener(void* callback)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return cvarmanager->AddGlobalChangeListener([callback](const char* convarName, int slot, const char* newValue, const char* oldValue) -> void {
        ((void(*)(const char*, int, const char*, const char*))callback)(convarName, slot, newValue, oldValue);
    });
}

void Bridge_Convars_RemoveGlobalChangeListener(uint64_t listenerID)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    cvarmanager->RemoveGlobalChangeListener(listenerID);
}

uint64_t Bridge_Convars_AddConvarCreatedListener(void* callback)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return cvarmanager->AddConvarCreatedListener([callback](const char* convarName) -> void {
        ((void(*)(const char*))callback)(convarName);
    });
}

void Bridge_Convars_RemoveConvarCreatedListener(uint64_t listenerID)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    cvarmanager->RemoveConvarCreatedListener(listenerID);
}

uint64_t Bridge_Convars_AddConCommandCreatedListener(void* callback)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    return cvarmanager->AddConCommandCreatedListener([callback](const char* convarName) -> void {
        ((void(*)(const char*))callback)(convarName);
    });
}

void Bridge_Convars_RemoveConCommandCreatedListener(uint64_t listenerID)
{
    auto cvarmanager = g_ifaceService.FetchInterface<IConvarManager>(CONVARMANAGER_INTERFACE_VERSION);
    cvarmanager->RemoveConCommandCreatedListener(listenerID);
}

DEFINE_NATIVE("Convars.QueryClientConvar", Bridge_Convars_QueryClientConvar);
DEFINE_NATIVE("Convars.AddQueryClientCvarCallback", Bridge_Convars_AddQueryClientCvarCallback);
DEFINE_NATIVE("Convars.RemoveQueryClientCvarCallback", Bridge_Convars_RemoveQueryClientCvarCallback);
DEFINE_NATIVE("Convars.AddGlobalChangeListener", Bridge_Convars_AddGlobalChangeListener);
DEFINE_NATIVE("Convars.RemoveGlobalChangeListener", Bridge_Convars_RemoveGlobalChangeListener);
DEFINE_NATIVE("Convars.AddConvarCreatedListener", Bridge_Convars_AddConvarCreatedListener);
DEFINE_NATIVE("Convars.RemoveConvarCreatedListener", Bridge_Convars_RemoveConvarCreatedListener);
DEFINE_NATIVE("Convars.AddConCommandCreatedListener", Bridge_Convars_AddConCommandCreatedListener);
DEFINE_NATIVE("Convars.RemoveConCommandCreatedListener", Bridge_Convars_RemoveConCommandCreatedListener);
DEFINE_NATIVE("Convars.CreateConvarInt16", Bridge_Convars_CreateConvarInt16);
DEFINE_NATIVE("Convars.CreateConvarUInt16", Bridge_Convars_CreateConvarUInt16);
DEFINE_NATIVE("Convars.CreateConvarInt32", Bridge_Convars_CreateConvarInt32);
DEFINE_NATIVE("Convars.CreateConvarUInt32", Bridge_Convars_CreateConvarUInt32);
DEFINE_NATIVE("Convars.CreateConvarInt64", Bridge_Convars_CreateConvarInt64);
DEFINE_NATIVE("Convars.CreateConvarUInt64", Bridge_Convars_CreateConvarUInt64);
DEFINE_NATIVE("Convars.CreateConvarBool", Bridge_Convars_CreateConvarBool);
DEFINE_NATIVE("Convars.CreateConvarFloat", Bridge_Convars_CreateConvarFloat);
DEFINE_NATIVE("Convars.CreateConvarDouble", Bridge_Convars_CreateConvarDouble);
DEFINE_NATIVE("Convars.CreateConvarColor", Bridge_Convars_CreateConvarColor);
DEFINE_NATIVE("Convars.CreateConvarVector2D", Bridge_Convars_CreateConvarVector2D);
DEFINE_NATIVE("Convars.CreateConvarVector", Bridge_Convars_CreateConvarVector);
DEFINE_NATIVE("Convars.CreateConvarVector4D", Bridge_Convars_CreateConvarVector4D);
DEFINE_NATIVE("Convars.CreateConvarQAngle", Bridge_Convars_CreateConvarQAngle);
DEFINE_NATIVE("Convars.CreateConvarString", Bridge_Convars_CreateConvarString);
DEFINE_NATIVE("Convars.DeleteConvar", Bridge_Convars_DeleteConvar);
DEFINE_NATIVE("Convars.ExistsConvar", Bridge_Convars_ExistsConvar);
DEFINE_NATIVE("Convars.GetConvarType", Bridge_Convars_GetConvarType);
DEFINE_NATIVE("Convars.GetConvarDataAddress", Bridge_Convars_GetConvarDataAddress);
DEFINE_NATIVE("Convars.GetConvarValueInt16", Bridge_Convars_GetConvarValueInt16);
DEFINE_NATIVE("Convars.GetConvarValueUInt16", Bridge_Convars_GetConvarValueUInt16);
DEFINE_NATIVE("Convars.GetConvarValueInt32", Bridge_Convars_GetConvarValueInt32);
DEFINE_NATIVE("Convars.GetConvarValueUInt32", Bridge_Convars_GetConvarValueUInt32);
DEFINE_NATIVE("Convars.GetConvarValueInt64", Bridge_Convars_GetConvarValueInt64);
DEFINE_NATIVE("Convars.GetConvarValueUInt64", Bridge_Convars_GetConvarValueUInt64);
DEFINE_NATIVE("Convars.GetConvarValueBool", Bridge_Convars_GetConvarValueBool);
DEFINE_NATIVE("Convars.GetConvarValueFloat", Bridge_Convars_GetConvarValueFloat);
DEFINE_NATIVE("Convars.GetConvarValueDouble", Bridge_Convars_GetConvarValueDouble);
DEFINE_NATIVE("Convars.GetConvarValueColor", Bridge_Convars_GetConvarValueColor);
DEFINE_NATIVE("Convars.GetConvarValueVector2D", Bridge_Convars_GetConvarValueVector2D);
DEFINE_NATIVE("Convars.GetConvarValueVector", Bridge_Convars_GetConvarValueVector);
DEFINE_NATIVE("Convars.GetConvarValueVector4D", Bridge_Convars_GetConvarValueVector4D);
DEFINE_NATIVE("Convars.GetConvarValueQAngle", Bridge_Convars_GetConvarValueQAngle);
DEFINE_NATIVE("Convars.GetConvarValueString", Bridge_Convars_GetConvarValueString);
DEFINE_NATIVE("Convars.SetConvarValueInt16", Bridge_Convars_SetConvarValueInt16);
DEFINE_NATIVE("Convars.SetConvarValueUInt16", Bridge_Convars_SetConvarValueUInt16);
DEFINE_NATIVE("Convars.SetConvarValueInt32", Bridge_Convars_SetConvarValueInt32);
DEFINE_NATIVE("Convars.SetConvarValueUInt32", Bridge_Convars_SetConvarValueUInt32);
DEFINE_NATIVE("Convars.SetConvarValueInt64", Bridge_Convars_SetConvarValueInt64);
DEFINE_NATIVE("Convars.SetConvarValueUInt64", Bridge_Convars_SetConvarValueUInt64);
DEFINE_NATIVE("Convars.SetConvarValueBool", Bridge_Convars_SetConvarValueBool);
DEFINE_NATIVE("Convars.SetConvarValueFloat", Bridge_Convars_SetConvarValueFloat);
DEFINE_NATIVE("Convars.SetConvarValueDouble", Bridge_Convars_SetConvarValueDouble);
DEFINE_NATIVE("Convars.SetConvarValueColor", Bridge_Convars_SetConvarValueColor);
DEFINE_NATIVE("Convars.SetConvarValueVector2D", Bridge_Convars_SetConvarValueVector2D);
DEFINE_NATIVE("Convars.SetConvarValueVector", Bridge_Convars_SetConvarValueVector);
DEFINE_NATIVE("Convars.SetConvarValueVector4D", Bridge_Convars_SetConvarValueVector4D);
DEFINE_NATIVE("Convars.SetConvarValueQAngle", Bridge_Convars_SetConvarValueQAngle);
DEFINE_NATIVE("Convars.SetConvarValueString", Bridge_Convars_SetConvarValueString);
DEFINE_NATIVE("Convars.SetClientConvarValueInt16", Bridge_Convars_SetClientConvarValueInt16);
DEFINE_NATIVE("Convars.SetClientConvarValueUInt16", Bridge_Convars_SetClientConvarValueUInt16);
DEFINE_NATIVE("Convars.SetClientConvarValueInt32", Bridge_Convars_SetClientConvarValueInt32);
DEFINE_NATIVE("Convars.SetClientConvarValueUInt32", Bridge_Convars_SetClientConvarValueUInt32);
DEFINE_NATIVE("Convars.SetClientConvarValueInt64", Bridge_Convars_SetClientConvarValueInt64);
DEFINE_NATIVE("Convars.SetClientConvarValueUInt64", Bridge_Convars_SetClientConvarValueUInt64);
DEFINE_NATIVE("Convars.SetClientConvarValueBool", Bridge_Convars_SetClientConvarValueBool);
DEFINE_NATIVE("Convars.SetClientConvarValueFloat", Bridge_Convars_SetClientConvarValueFloat);
DEFINE_NATIVE("Convars.SetClientConvarValueDouble", Bridge_Convars_SetClientConvarValueDouble);
DEFINE_NATIVE("Convars.SetClientConvarValueColor", Bridge_Convars_SetClientConvarValueColor);
DEFINE_NATIVE("Convars.SetClientConvarValueVector2D", Bridge_Convars_SetClientConvarValueVector2D);
DEFINE_NATIVE("Convars.SetClientConvarValueVector", Bridge_Convars_SetClientConvarValueVector);
DEFINE_NATIVE("Convars.SetClientConvarValueVector4D", Bridge_Convars_SetClientConvarValueVector4D);
DEFINE_NATIVE("Convars.SetClientConvarValueQAngle", Bridge_Convars_SetClientConvarValueQAngle);
DEFINE_NATIVE("Convars.SetClientConvarValueString", Bridge_Convars_SetClientConvarValueString);
DEFINE_NATIVE("Convars.AddFlags", Bridge_Convars_AddFlags);
DEFINE_NATIVE("Convars.RemoveFlags", Bridge_Convars_RemoveFlags);
DEFINE_NATIVE("Convars.ClearFlags", Bridge_Convars_ClearFlags);
DEFINE_NATIVE("Convars.GetFlags", Bridge_Convars_GetFlags);