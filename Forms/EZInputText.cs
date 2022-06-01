// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using EZAccess.Blazor.Table;
using Microsoft.AspNetCore.Components.Rendering;

namespace EZAccess.Blazor.Forms;

/* This is almost equivalent to a .razor file containing:
 *
 *    @inherits InputBase<string>
 *    <input @bind="CurrentValue" id="@Id" class="@CssClass" />
 *
 * The only reason it's not implemented as a .razor file is that we don't presently have the ability to compile those
 * files within this project. Developers building their own input components should use Razor syntax.
 */

/// <summary>
/// An input component for editing <see cref="string"/> values.
/// </summary>
public class EZInputText<TValue> : InputBase<TValue?>
{
    [Parameter] public string? Id { get; set; }
    [Parameter] public bool Editable { get; set; }
    [Parameter] public Expression<Func<TValue?>>? ValueExpressionOverwrite { get; set; }
    protected string ParsingErrorMessage { get; set; } = "The {0} field must be text.";

    /// <summary>
    /// Overwrite the parameters to get the correct ValueExpression, which list used to determine the FieldIdentifier
    /// </summary>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        if (ValueExpressionOverwrite != null)
        {
            ValueExpression = ValueExpressionOverwrite;
        }
        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override void OnInitialized()
    {
        Id ??= FieldIdentifier.FieldName;
    }

    /// <summary>
    /// Gets or sets the associated <see cref="ElementReference"/>.
    /// <para>
    /// May be <see langword="null"/> if accessed before the component is rendered.
    /// </para>
    /// </summary>
    [DisallowNull] public ElementReference? Element { get; protected set; }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddAttribute(1, "autocomplete", "off");
        if (!Editable)
        {
            builder.AddAttribute(2, "readonly");
        }
        builder.AddMultipleAttributes(3, AdditionalAttributes);
        builder.AddAttributeIfNotNullOrEmpty(4, "class", CssClass);
        builder.AddAttribute(5, "id", Id);
        builder.AddAttribute(6, "value", BindConverter.FormatValue(CurrentValue));
        builder.AddAttribute(7, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        builder.AddElementReferenceCapture(8, __inputReference => Element = __inputReference);
        builder.CloseElement();
    }
    //<input autocomplete="off" type="text" @bind="CurrentValue" @bind:event="onchange" id="@Id" class="@CssClass" readonly="@(!Editable)" />

    /// <inheritdoc />
    //protected override bool TryParseValueFromString(string? value, out string? result, [NotNullWhen(false)] out string? validationErrorMessage)
    //{
    //    result = value;
    //    validationErrorMessage = null;
    //    return true;
    //}
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result))
        {
            validationErrorMessage = null;
            return true;
        }
        else
        {
            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, ParsingErrorMessage, DisplayName ?? FieldIdentifier.FieldName);
            return false;
        }
    }
}