test_func ; start subroutine
	lda $health
	pha
	lda #0
	pha
	; building == statement
	pla
	tax ; move operand out of the way
	pla
	sta $temp
	txa ; move operand back
	cmp $temp
	php ; flags -> a
	pla
	srl; put z bit in rightmost place
	and #1; mask z bit
	pha ; push result of == 
	pla ; pop condition
	cmp #1
	beq 3 ; enter if-body
	jmp _B
	lda $gameover
	pha
	lda #0
	pha
	; building == statement
	pla
	tax ; move operand out of the way
	pla
	sta $temp
	txa ; move operand back
	cmp $temp
	php ; flags -> a
	pla
	srl; put z bit in rightmost place
	and #1; mask z bit
	pha ; push result of == 
	pla ; pop condition
	cmp #1
	beq 3 ; enter if-body
	jmp _D
	lda #55
	pha
	;variable assignment
	pla
	sta $health
_D
_C
	lda #1
	pha
	; building set attr call
	pla ; attr val -> x
	tax
	lda #1; attr # -> y (holder)
	tay ; 
	lda #1 ; target object is player
	jsr set_obj_attr
	jmp _A
_B
	lda $health
	pha
	lda #1
	pha
	; building == statement
	pla
	tax ; move operand out of the way
	pla
	sta $temp
	txa ; move operand back
	cmp $temp
	php ; flags -> a
	pla
	srl; put z bit in rightmost place
	and #1; mask z bit
	pha ; push result of == 
	pla ; pop condition
	cmp #1
	beq 3 ; enter if-body
	jmp _E
	lda #2
	pha
	; building set attr call
	pla ; attr val -> x
	tax
	lda #1; attr # -> y (holder)
	tay ; 
	lda #1 ; target object is player
	jsr set_obj_attr
	jmp _A
_E
	;building a println statement
	lda #$string_table%256
	sta $tableAddr
	lda #$string_table/256
	sta $tableAddr+1
	lda #0 ; "hi"
	jsr printix
	jsr printcr ; print newline
_A
	rts
