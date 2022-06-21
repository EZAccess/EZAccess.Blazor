﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components.Rendering;
using System.Linq.Expressions;

namespace EZAccess.Blazor.Forms;

/// <summary>
/// An input component for editing numeric values.
/// Supported numeric types are <see cref="int"/>, <see cref="long"/>, <see cref="short"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>.
/// </summary>
public class EZInputNumber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue?>
{
    [CascadingParameter(Name = "ActionOnFocus")] private Action<bool>? ActionOnFocus { get; set; }

    private static readonly string _stepAttributeValue = GetStepAttributeValue();

    private static string GetStepAttributeValue()
    {
        // Unwrap Nullable<T>, because InputBase already deals with the Nullable aspect
        // of it for us. We will only get asked to parse the T for nonempty inputs.
        var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
        if (targetType == typeof(int) ||
            targetType == typeof(long) ||
            targetType == typeof(short) ||
            targetType == typeof(float) ||
            targetType == typeof(double) ||
            targetType == typeof(decimal))
        {
            return "any";
        }
        else
        {
            throw new InvalidOperationException($"The type '{targetType}' is not a supported numeric type.");
        }
    }

    /// <summary>
    /// Gets or sets the error message used when displaying an a parsing error.
    /// </summary>
    [Parameter] public string ParsingErrorMessage { get; set; } = "The {0} field must be a number.";
    [Parameter] public bool AllowEdit { get; set; }
    [Parameter] public Expression<Func<TValue?>>? ValueExpressionOverwrite { get; set; }
    [Parameter] public string? Id { get; set; }

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
        builder.OpenElement(1, "input");
        builder.AddAttribute(2, "autocomplete", "off");
        if (!AllowEdit)
        {
            builder.AddAttribute(3, "readonly");
        }
        builder.AddAttribute(4, "step", _stepAttributeValue);
        builder.AddMultipleAttributes(5, AdditionalAttributes);
        builder.AddAttribute(6, "type", "number");
        builder.AddAttributeIfNotNullOrEmpty(7, "class", CssClass);
        builder.AddAttribute(8, "id", Id);
        builder.AddAttribute(9, "value", BindConverter.FormatValue(CurrentValueAsString));
        //builder.AddAttribute(helper.Seq, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        builder.AddAttribute(10, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        builder.AddAttribute(11, "onfocus", EventCallback.Factory.Create(this, OnFocus));
        builder.AddElementReferenceCapture(12, __inputReference => Element = __inputReference);

        builder.CloseElement();

    }

    protected override void OnInitialized()
    {
        Id ??= FieldIdentifier.FieldName;
        base.OnInitialized();
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

    /// <inheritdoc />
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

    /// <summary>
    /// Formats the value as a string. Derived classes can override this to determine the formatting used for <c>CurrentValueAsString</c>.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A string representation of the value.</returns>
    protected override string? FormatValueAsString(TValue? value)
    {
        // Avoiding a cast to IFormattable to avoid boxing.
        switch (value)
        {
            case null:
                return null;

            case int @int:
                return BindConverter.FormatValue(@int, CultureInfo.InvariantCulture);

            case long @long:
                return BindConverter.FormatValue(@long, CultureInfo.InvariantCulture);

            case short @short:
                return BindConverter.FormatValue(@short, CultureInfo.InvariantCulture);

            case float @float:
                return BindConverter.FormatValue(@float, CultureInfo.InvariantCulture);

            case double @double:
                return BindConverter.FormatValue(@double, CultureInfo.InvariantCulture);

            case decimal @decimal:
                return BindConverter.FormatValue(@decimal, CultureInfo.InvariantCulture);

            default:
                throw new InvalidOperationException($"Unsupported type {value.GetType()}");
        }
    }

    private void OnFocus()
    {
        ActionOnFocus?.Invoke(false);
    }

}