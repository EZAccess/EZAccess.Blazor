// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Rendering;

namespace EZAccess.Blazor.Forms;

/// <summary>
/// An input component for editing <see cref="bool"/> values.
/// </summary>
public class EZInputCheckbox<TValue> : InputBase<TValue?>
{
    [CascadingParameter(Name = "ActionOnFocus")] private Action<bool>? ActionOnFocus { get; set; }
    [Parameter] public Expression<Func<TValue?>>? ValueExpressionOverwrite { get; set; }
    [Parameter] public string? Id { get; set; }
    [Parameter] public bool Editable { get; set; }

    protected override void OnInitialized()
    {
        Id ??= FieldIdentifier.FieldName;
    }

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
        string cssClass = "form-check-input";
        builder.OpenElement(0, "input");
        if (!Editable)
        {
            builder.AddAttribute(1, "readonly");
        }
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddAttribute(3, "type", "checkbox");
        builder.AddAttribute(4, "class", cssClass);
        builder.AddAttribute(5, "id", Id);
        builder.AddAttribute(6, "checked", BindConverter.FormatValue(CurrentValue));
        builder.AddAttribute(7, "onchange", EventCallback.Factory.CreateBinder(this, __value => CurrentValue = __value, CurrentValue));
        builder.AddAttribute(8, "onfocus", EventCallback.Factory.Create(this, OnFocus));
        builder.AddElementReferenceCapture(9, __inputReference => Element = __inputReference);
        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotSupportedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
    }
    //protected override bool TryParseValueFromString(string? value, out bool result, [NotNullWhen(false)] out string? validationErrorMessage)
    //    => throw new NotSupportedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");

    private void OnFocus()
    {
        ActionOnFocus?.Invoke(false);
    }

}